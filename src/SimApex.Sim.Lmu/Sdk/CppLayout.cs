using System.Globalization;
using System.Text;

namespace SimApex.Sim.Lmu.Sdk;

/// <summary>Primitive storage kinds the layout engine knows how to size and read.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "The names describe C++ storage types on purpose.")]
public enum PrimitiveKind
{
    None,
    Int8,
    UInt8,
    Bool,
    Char,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Float32,
    Float64,
    Pointer,
}

/// <summary>One field of a computed C++ type layout.</summary>
/// <param name="Name">Member name (members of anonymous unions/structs are flattened into the parent).</param>
/// <param name="Offset">Byte offset from the start of the containing type.</param>
/// <param name="ElementSize">Size in bytes of one element.</param>
/// <param name="Kind">Primitive kind, or <see cref="PrimitiveKind.None"/> for a struct-typed field.</param>
/// <param name="TypeName">C++ type name as declared (struct or enum name for non-primitives).</param>
/// <param name="Dimensions">Array dimensions, empty for scalars.</param>
public sealed record FieldLayout(string Name, int Offset, int ElementSize, PrimitiveKind Kind, string TypeName, IReadOnlyList<int> Dimensions)
{
    public int ElementCount => Dimensions.Aggregate(1, (a, d) => a * d);

    public int TotalSize => ElementSize * ElementCount;
}

/// <summary>Computed size, alignment and fields of one C++ struct, class or union.</summary>
public sealed record TypeLayout(string Name, int Size, int Alignment, IReadOnlyList<FieldLayout> Fields)
{
    public FieldLayout? Find(string name) => Fields.FirstOrDefault(f => f.Name == name);
}

/// <summary>
/// Minimal C++ header reader that computes struct layouts with the rules of the Microsoft x64
/// compiler: natural alignment capped by <c>#pragma pack</c>, default packing 8, <c>long</c> = 4
/// bytes, pointers = 8 bytes, a vtable pointer for classes with virtual members, no reuse of tail
/// padding, anonymous unions and structs flattened into their parent.
/// <para>
/// It understands the subset of C++ used by plugin SDK headers (struct/class/union/enum definitions,
/// member functions, constructors with initialiser lists, static functions, <c>#pragma pack</c>,
/// numeric <c>#define</c>s). It is not a general C++ parser: unknown constructs throw
/// <see cref="FormatException"/> rather than producing a silently wrong layout.
/// </para>
/// </summary>
public sealed class CppLayoutEngine
{
    private const int DefaultPack = 8;

    private readonly Dictionary<string, Aggregate> _aggregates = new(StringComparer.Ordinal);
    private readonly Dictionary<string, PrimitiveKind> _enums = new(StringComparer.Ordinal);
    private readonly Dictionary<string, long> _constants = new(StringComparer.Ordinal) { ["MAX_PATH"] = 260 };
    private readonly Dictionary<string, TypeLayout> _layoutCache = new(StringComparer.Ordinal);
    private readonly Stack<int> _packStack = new();
    private int _pack = DefaultPack;

    /// <summary>Names of all aggregates parsed so far.</summary>
    public IEnumerable<string> TypeNames => _aggregates.Keys;

    /// <summary>Parses one header. Call in include order; packing state carries over between calls.</summary>
    public void AddHeader(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var parser = new Parser(this, Tokenizer.Tokenize(source, this));
        parser.ParseTopLevel();
        _layoutCache.Clear();
    }

    /// <summary>Value of a numeric <c>#define</c> or enum constant (e.g. <c>MAX_PATH</c>, <c>SharedMemoryEvent::SME_MAX</c>).</summary>
    public long GetConstant(string name) =>
        _constants.TryGetValue(name, out long v) ? v : throw new KeyNotFoundException($"Unknown constant '{name}'.");

    /// <summary>Computes (and caches) the layout of a named struct, class or union.</summary>
    public TypeLayout GetLayout(string typeName)
    {
        if (_layoutCache.TryGetValue(typeName, out var cached))
        {
            return cached;
        }

        if (!_aggregates.TryGetValue(typeName, out var aggregate))
        {
            throw new KeyNotFoundException($"Type '{typeName}' was not found in the parsed headers.");
        }

        var layout = Compute(aggregate, typeName);
        _layoutCache[typeName] = layout;
        return layout;
    }

    private TypeLayout Compute(Aggregate a, string name)
    {
        var fields = new List<FieldLayout>();
        int offset = 0;
        int align = 1;
        bool hasVptr = false;

        foreach (string baseName in a.Bases)
        {
            var b = GetLayout(baseName);
            int at = AlignUp(offset, Math.Min(b.Alignment, a.Pack));
            fields.AddRange(b.Fields.Select(f => f with { Offset = f.Offset + at }));
            offset = at + b.Size;
            align = Math.Max(align, Math.Min(b.Alignment, a.Pack));
            hasVptr |= _aggregates[baseName].HasVirtualInHierarchy(this);
        }

        if (a.HasVirtual && !hasVptr)
        {
            // MSVC places the vtable pointer first when no base class already has one.
            if (a.Bases.Count > 0)
            {
                throw new FormatException($"'{name}': virtual members added to a non-polymorphic base are not supported.");
            }

            offset = 8;
            align = Math.Max(align, Math.Min(8, a.Pack));
        }

        foreach (var member in a.Members)
        {
            int size;
            int memberAlign;
            IEnumerable<FieldLayout> placed;
            if (member.Anonymous is { } nested)
            {
                var inner = Compute(nested, name + ".<anonymous>");
                size = inner.Size;
                memberAlign = Math.Min(inner.Alignment, a.Pack);
                int at = a.IsUnion ? 0 : AlignUp(offset, memberAlign);
                placed = inner.Fields.Select(f => f with { Offset = f.Offset + at });
                offset = a.IsUnion ? Math.Max(offset, size) : at + size;
            }
            else
            {
                var (elementSize, natural, kind) = SizeOf(member.TypeName, member.IsPointer);
                int count = member.Dimensions.Aggregate(1, (x, d) => x * d);
                size = elementSize * count;
                memberAlign = Math.Min(natural, a.Pack);
                int at = a.IsUnion ? 0 : AlignUp(offset, memberAlign);
                placed = [new FieldLayout(member.Name!, at, elementSize, kind, member.IsPointer ? member.TypeName + "*" : member.TypeName, member.Dimensions)];
                offset = a.IsUnion ? Math.Max(offset, size) : at + size;
            }

            fields.AddRange(placed);
            align = Math.Max(align, memberAlign);
        }

        int total = Math.Max(1, AlignUp(offset, align));
        return new TypeLayout(name, total, align, fields);
    }

    private (int Size, int Align, PrimitiveKind Kind) SizeOf(string typeName, bool isPointer)
    {
        if (isPointer)
        {
            return (8, 8, PrimitiveKind.Pointer);
        }

        if (Primitives.TryGetValue(typeName, out var p))
        {
            return p;
        }

        if (_enums.TryGetValue(typeName, out var underlying))
        {
            int s = KindSize(underlying);
            return (s, s, underlying);
        }

        if (_aggregates.ContainsKey(typeName))
        {
            var l = GetLayout(typeName);
            return (l.Size, l.Alignment, PrimitiveKind.None);
        }

        throw new FormatException($"Unknown type '{typeName}': add it to the primitive table or parse the header that defines it.");
    }

    private static int AlignUp(int value, int alignment) => (value + alignment - 1) / alignment * alignment;

    private static int KindSize(PrimitiveKind k) => k switch
    {
        PrimitiveKind.Int8 or PrimitiveKind.UInt8 or PrimitiveKind.Bool or PrimitiveKind.Char => 1,
        PrimitiveKind.Int16 or PrimitiveKind.UInt16 => 2,
        PrimitiveKind.Int32 or PrimitiveKind.UInt32 or PrimitiveKind.Float32 => 4,
        _ => 8,
    };

    // Microsoft x64 (LLP64) sizes. Windows handle types are pointers.
    private static readonly Dictionary<string, (int, int, PrimitiveKind)> Primitives = new(StringComparer.Ordinal)
    {
        ["char"] = (1, 1, PrimitiveKind.Char),
        ["signed char"] = (1, 1, PrimitiveKind.Int8),
        ["unsigned char"] = (1, 1, PrimitiveKind.UInt8),
        ["bool"] = (1, 1, PrimitiveKind.Bool),
        ["int8_t"] = (1, 1, PrimitiveKind.Int8),
        ["uint8_t"] = (1, 1, PrimitiveKind.UInt8),
        ["BYTE"] = (1, 1, PrimitiveKind.UInt8),
        ["short"] = (2, 2, PrimitiveKind.Int16),
        ["unsigned short"] = (2, 2, PrimitiveKind.UInt16),
        ["int16_t"] = (2, 2, PrimitiveKind.Int16),
        ["uint16_t"] = (2, 2, PrimitiveKind.UInt16),
        ["WORD"] = (2, 2, PrimitiveKind.UInt16),
        ["int"] = (4, 4, PrimitiveKind.Int32),
        ["unsigned int"] = (4, 4, PrimitiveKind.UInt32),
        ["long"] = (4, 4, PrimitiveKind.Int32),
        ["unsigned long"] = (4, 4, PrimitiveKind.UInt32),
        ["int32_t"] = (4, 4, PrimitiveKind.Int32),
        ["uint32_t"] = (4, 4, PrimitiveKind.UInt32),
        ["LONG"] = (4, 4, PrimitiveKind.Int32),
        ["ULONG"] = (4, 4, PrimitiveKind.UInt32),
        ["DWORD"] = (4, 4, PrimitiveKind.UInt32),
        ["BOOL"] = (4, 4, PrimitiveKind.Int32),
        ["UINT"] = (4, 4, PrimitiveKind.UInt32),
        ["float"] = (4, 4, PrimitiveKind.Float32),
        ["float_t"] = (4, 4, PrimitiveKind.Float32),
        ["long long"] = (8, 8, PrimitiveKind.Int64),
        ["unsigned long long"] = (8, 8, PrimitiveKind.UInt64),
        ["int64_t"] = (8, 8, PrimitiveKind.Int64),
        ["uint64_t"] = (8, 8, PrimitiveKind.UInt64),
        ["size_t"] = (8, 8, PrimitiveKind.UInt64),
        ["ptrdiff_t"] = (8, 8, PrimitiveKind.Int64),
        ["intptr_t"] = (8, 8, PrimitiveKind.Int64),
        ["uintptr_t"] = (8, 8, PrimitiveKind.UInt64),
        ["double"] = (8, 8, PrimitiveKind.Float64),
        ["double_t"] = (8, 8, PrimitiveKind.Float64),
        ["long double"] = (8, 8, PrimitiveKind.Float64),
        ["HANDLE"] = (8, 8, PrimitiveKind.Pointer),
        ["HWND"] = (8, 8, PrimitiveKind.Pointer),
        ["HMODULE"] = (8, 8, PrimitiveKind.Pointer),
        ["HINSTANCE"] = (8, 8, PrimitiveKind.Pointer),
        ["LPVOID"] = (8, 8, PrimitiveKind.Pointer),
        ["PVOID"] = (8, 8, PrimitiveKind.Pointer),
    };

    private sealed class Aggregate(bool isUnion, int pack)
    {
        public bool IsUnion { get; } = isUnion;

        public int Pack { get; } = pack;

        public bool HasVirtual { get; set; }

        public List<string> Bases { get; } = [];

        public List<Member> Members { get; } = [];

        public bool HasVirtualInHierarchy(CppLayoutEngine engine) =>
            HasVirtual || Bases.Any(b => engine._aggregates[b].HasVirtualInHierarchy(engine));
    }

    private sealed record Member(string? Name, string TypeName, bool IsPointer, IReadOnlyList<int> Dimensions, Aggregate? Anonymous);

    private static class Tokenizer
    {
        public static List<string> Tokenize(string s, CppLayoutEngine engine)
        {
            var tokens = new List<string>();
            int i = 0;
            bool lineStart = true;
            while (i < s.Length)
            {
                char c = s[i];
                if (c == '\n')
                {
                    lineStart = true;
                    i++;
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (c == '/' && i + 1 < s.Length && s[i + 1] == '/')
                {
                    while (i < s.Length && s[i] != '\n')
                    {
                        i++;
                    }

                    continue;
                }

                if (c == '/' && i + 1 < s.Length && s[i + 1] == '*')
                {
                    int end = s.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    i = end < 0 ? s.Length : end + 2;
                    continue;
                }

                if (c == '#' && lineStart)
                {
                    var line = new StringBuilder();
                    while (i < s.Length && s[i] != '\n')
                    {
                        if (s[i] == '\\' && i + 1 < s.Length && (s[i + 1] == '\n' || s[i + 1] == '\r'))
                        {
                            i += s[i + 1] == '\r' ? 3 : 2;
                            continue;
                        }

                        if (s[i] == '/' && i + 1 < s.Length && s[i + 1] == '/')
                        {
                            while (i < s.Length && s[i] != '\n')
                            {
                                i++;
                            }

                            break;
                        }

                        line.Append(s[i]);
                        i++;
                    }

                    string? directive = Directive(line.ToString(), engine);
                    if (directive is not null)
                    {
                        tokens.Add(directive);
                    }

                    continue;
                }

                lineStart = false;
                if (c == '"' || c == '\'')
                {
                    int start = i++;
                    while (i < s.Length && s[i] != c)
                    {
                        i += s[i] == '\\' ? 2 : 1;
                    }

                    i++;
                    tokens.Add(s[start..Math.Min(i, s.Length)]);
                    continue;
                }

                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_'))
                    {
                        i++;
                    }

                    tokens.Add(s[start..i]);
                    continue;
                }

                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '.'))
                    {
                        i++;
                    }

                    tokens.Add(s[start..i]);
                    continue;
                }

                if (i + 1 < s.Length && (s.AsSpan(i, 2) is "::" or "&&" or "->"))
                {
                    tokens.Add(s.Substring(i, 2));
                    i += 2;
                    continue;
                }

                tokens.Add(c.ToString());
                i++;
            }

            return tokens;
        }

        // Returns a pack token, or null for directives with no effect on layout.
        private static string? Directive(string line, CppLayoutEngine engine)
        {
            string body = line.TrimStart('#').Trim();
            if (body.StartsWith("define", StringComparison.Ordinal))
            {
                string[] parts = body.Split((char[]?)null, 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3 && TryParseInteger(parts[2].Trim().Trim('(', ')'), out long value))
                {
                    engine._constants[parts[1]] = value;
                }

                return null;
            }

            if (!body.StartsWith("pragma", StringComparison.Ordinal))
            {
                return null;
            }

            string compact = body.Replace(" ", "", StringComparison.Ordinal).Replace("\t", "", StringComparison.Ordinal);
            if (!compact.StartsWith("pragmapack(", StringComparison.Ordinal))
            {
                return null;
            }

            string args = compact["pragmapack(".Length..].TrimEnd(')');
            return "#pack:" + args;
        }
    }

    internal static bool TryParseInteger(string text, out long value)
    {
        string t = text.TrimEnd('u', 'U', 'l', 'L');
        if (t.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return long.TryParse(t[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        return long.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    private sealed class Parser(CppLayoutEngine engine, List<string> tokens)
    {
        private static readonly HashSet<string> TypeKeywords = new(StringComparer.Ordinal)
        {
            "unsigned", "signed", "short", "long", "int", "char", "bool", "float", "double", "void",
        };

        private static readonly HashSet<string> Qualifiers = new(StringComparer.Ordinal)
        {
            "const", "volatile", "mutable", "inline", "constexpr", "struct", "class", "union", "enum", "typename",
        };

        private int _i;

        private string Peek(int ahead = 0) => _i + ahead < tokens.Count ? tokens[_i + ahead] : "";

        public void ParseTopLevel()
        {
            while (_i < tokens.Count)
            {
                string t = Peek();
                if (TryDirective())
                {
                    continue;
                }

                if (t is "struct" or "class" or "union")
                {
                    if (IsDefinitionAhead())
                    {
                        ParseAggregateDefinition();
                        SkipTrailingDeclarators();
                    }
                    else
                    {
                        SkipDeclaration();
                    }
                }
                else if (t == "enum")
                {
                    ParseEnum();
                    SkipTrailingDeclarators();
                }
                else if (t == ";")
                {
                    _i++;
                }
                else
                {
                    SkipDeclaration();
                }
            }
        }

        private bool TryDirective()
        {
            string t = Peek();
            if (!t.StartsWith("#pack:", StringComparison.Ordinal))
            {
                return false;
            }

            string args = t["#pack:".Length..];
            string[] parts = args.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0)
            {
                engine._pack = DefaultPack;
            }
            else if (parts[0] == "push")
            {
                engine._packStack.Push(engine._pack);
                if (parts.Length > 1 && TryParseInteger(parts[^1], out long n))
                {
                    engine._pack = (int)n;
                }
            }
            else if (parts[0] == "pop")
            {
                engine._pack = engine._packStack.Count > 0 ? engine._packStack.Pop() : DefaultPack;
            }
            else if (TryParseInteger(parts[0], out long n))
            {
                engine._pack = (int)n;
            }

            _i++;
            return true;
        }

        // At "struct|class|union": is this a definition (has a body) rather than a use or forward declaration?
        private bool IsDefinitionAhead()
        {
            int j = _i + 1;
            if (Peek(1) == "{")
            {
                return true;
            }

            if (!IsIdentifier(Peek(1)))
            {
                return false;
            }

            j = _i + 2;
            return j < tokens.Count && (tokens[j] == "{" || (tokens[j] == ":" && (j + 1 >= tokens.Count || tokens[j + 1] != ":")));
        }

        private Aggregate ParseAggregateDefinition()
        {
            bool isUnion = Peek() == "union";
            _i++;
            string? name = null;
            if (IsIdentifier(Peek()))
            {
                name = Peek();
                _i++;
            }

            var aggregate = new Aggregate(isUnion, engine._pack);
            if (Peek() == ":")
            {
                _i++;
                while (Peek() != "{")
                {
                    string t = Peek();
                    if (IsIdentifier(t) && t is not ("public" or "private" or "protected" or "virtual"))
                    {
                        aggregate.Bases.Add(t);
                    }

                    _i++;
                }
            }

            Expect("{");
            ParseBody(aggregate);
            Expect("}");
            if (name is not null)
            {
                engine._aggregates[name] = aggregate;
            }

            return aggregate;
        }

        private void ParseBody(Aggregate a)
        {
            while (Peek() != "}")
            {
                if (_i >= tokens.Count)
                {
                    throw new FormatException("Unexpected end of header inside a struct body.");
                }

                if (TryDirective())
                {
                    continue;
                }

                string t = Peek();
                if (t is "public" or "private" or "protected" && Peek(1) == ":")
                {
                    _i += 2;
                    continue;
                }

                if (t == ";")
                {
                    _i++;
                    continue;
                }

                if (t is "struct" or "class" or "union" && Peek(1) == "{")
                {
                    var nested = ParseAggregateDefinition();
                    if (IsIdentifier(Peek()))
                    {
                        throw new FormatException("Named members of anonymous aggregate type are not supported.");
                    }

                    a.Members.Add(new Member(null, "", false, [], nested));
                    Expect(";");
                    continue;
                }

                if (t is "struct" or "class" or "union" && IsDefinitionAhead())
                {
                    ParseAggregateDefinition();
                    SkipTrailingDeclarators();
                    continue;
                }

                if (t == "enum")
                {
                    ParseEnum();
                    SkipTrailingDeclarators();
                    continue;
                }

                if (t == "virtual")
                {
                    a.HasVirtual = true;
                    SkipDeclaration();
                    continue;
                }

                if (t is "static" or "friend" or "typedef" or "using" or "template" or "static_assert")
                {
                    SkipDeclaration();
                    continue;
                }

                if (IsFunctionAhead())
                {
                    SkipDeclaration();
                    continue;
                }

                ParseDataMembers(a);
            }
        }

        // Scans to the end of the declaration: a '(' at bracket depth 0 before '=', ';' or '{' means a function.
        private bool IsFunctionAhead()
        {
            int square = 0;
            for (int j = _i; j < tokens.Count; j++)
            {
                string t = tokens[j];
                if (t == "[")
                {
                    square++;
                }
                else if (t == "]")
                {
                    square--;
                }
                else if (square == 0 && t == "(")
                {
                    return true;
                }
                else if (square == 0 && t is "=" or ";" or "{" or "}")
                {
                    return false;
                }
            }

            return false;
        }

        private void ParseDataMembers(Aggregate a)
        {
            var typeParts = new List<string>();
            string? namedType = null;
            while (true)
            {
                string t = Peek();
                if (Qualifiers.Contains(t))
                {
                    _i++;
                }
                else if (TypeKeywords.Contains(t))
                {
                    typeParts.Add(t);
                    _i++;
                }
                else if (namedType is null && typeParts.Count == 0 && IsIdentifier(t))
                {
                    namedType = t;
                    _i++;
                    while (Peek() == "::" && IsIdentifier(Peek(1)))
                    {
                        namedType = Peek(1);
                        _i += 2;
                    }

                    if (Peek() == "<")
                    {
                        throw new FormatException($"Template member type '{namedType}<...>' is not supported.");
                    }
                }
                else
                {
                    break;
                }
            }

            string typeName = namedType ?? CanonicalKeywordType(typeParts);
            while (true)
            {
                bool pointer = false;
                while (Peek() is "*" or "&" or "&&" or "const" or "volatile")
                {
                    pointer |= Peek() is "*" or "&" or "&&";
                    _i++;
                }

                string name = Peek();
                if (!IsIdentifier(name))
                {
                    throw new FormatException($"Expected a member name after type '{typeName}', found '{name}'.");
                }

                _i++;
                var dims = new List<int>();
                while (Peek() == "[")
                {
                    _i++;
                    var expr = new List<string>();
                    while (Peek() != "]")
                    {
                        expr.Add(Peek());
                        _i++;
                    }

                    _i++;
                    dims.Add((int)Evaluate(expr));
                }

                if (Peek() == "=" || Peek() == "{")
                {
                    SkipInitializer();
                }

                a.Members.Add(new Member(name, typeName, pointer, dims, null));
                if (Peek() == ",")
                {
                    _i++;
                    continue;
                }

                Expect(";");
                return;
            }
        }

        private static string CanonicalKeywordType(List<string> parts)
        {
            int longs = parts.Count(p => p == "long");
            bool unsigned = parts.Contains("unsigned");
            bool signed = parts.Contains("signed");
            string? core = parts.FirstOrDefault(p => p is "char" or "short" or "int" or "bool" or "float" or "double" or "void");
            string baseName = core switch
            {
                "char" => signed ? "signed char" : "char",
                "short" => "short",
                "bool" => "bool",
                "float" => "float",
                "double" => longs > 0 ? "long double" : "double",
                "void" => "void",
                _ => longs switch { 0 => "int", 1 => "long", _ => "long long" },
            };
            if (core == "short")
            {
                baseName = "short";
            }
            else if (core == "int" && longs > 0)
            {
                baseName = longs == 1 ? "long" : "long long";
            }

            return unsigned ? "unsigned " + (baseName == "signed char" ? "char" : baseName) : baseName;
        }

        private void ParseEnum()
        {
            _i++;
            if (Peek() is "class" or "struct")
            {
                _i++;
            }

            string? name = IsIdentifier(Peek()) ? Peek() : null;
            if (name is not null)
            {
                _i++;
            }

            var underlying = PrimitiveKind.Int32;
            if (Peek() == ":")
            {
                _i++;
                var parts = new List<string>();
                while (Peek() is not "{" and not ";")
                {
                    parts.Add(Peek());
                    _i++;
                }

                string typeName = parts.Count == 1 && !TypeKeywords.Contains(parts[0]) ? parts[0] : CanonicalKeywordType(parts);
                underlying = Primitives.TryGetValue(typeName, out var p)
                    ? p.Item3
                    : throw new FormatException($"Unsupported enum underlying type '{typeName}'.");
            }

            if (Peek() == ";")
            {
                if (name is not null)
                {
                    engine._enums[name] = underlying;
                }

                return;
            }

            Expect("{");
            long next = 0;
            while (Peek() != "}")
            {
                string constant = Peek();
                _i++;
                long value = next;
                if (Peek() == "=")
                {
                    _i++;
                    var expr = new List<string>();
                    while (Peek() is not "," and not "}")
                    {
                        expr.Add(Peek());
                        _i++;
                    }

                    value = Evaluate(expr);
                }

                engine._constants[constant] = value;
                if (name is not null)
                {
                    engine._constants[name + "::" + constant] = value;
                }

                next = value + 1;
                if (Peek() == ",")
                {
                    _i++;
                }
            }

            Expect("}");
            if (name is not null)
            {
                engine._enums[name] = underlying;
            }
        }

        // Integer constant expressions: literals, known constants (optionally Enum::Name), + - * / and parentheses.
        private long Evaluate(List<string> expr)
        {
            var joined = new List<string>();
            for (int k = 0; k < expr.Count; k++)
            {
                if (k + 2 < expr.Count && expr[k + 1] == "::")
                {
                    joined.Add(expr[k] + "::" + expr[k + 2]);
                    k += 2;
                }
                else
                {
                    joined.Add(expr[k]);
                }
            }

            int pos = 0;
            long result = ParseSum(joined, ref pos);
            if (pos != joined.Count)
            {
                throw new FormatException($"Cannot evaluate constant expression '{string.Join(' ', expr)}'.");
            }

            return result;
        }

        private long ParseSum(List<string> e, ref int pos)
        {
            long v = ParseProduct(e, ref pos);
            while (pos < e.Count && e[pos] is "+" or "-")
            {
                string op = e[pos++];
                long r = ParseProduct(e, ref pos);
                v = op == "+" ? v + r : v - r;
            }

            return v;
        }

        private long ParseProduct(List<string> e, ref int pos)
        {
            long v = ParseAtom(e, ref pos);
            while (pos < e.Count && e[pos] is "*" or "/")
            {
                string op = e[pos++];
                long r = ParseAtom(e, ref pos);
                v = op == "*" ? v * r : v / r;
            }

            return v;
        }

        private long ParseAtom(List<string> e, ref int pos)
        {
            if (pos >= e.Count)
            {
                throw new FormatException("Truncated constant expression.");
            }

            string t = e[pos++];
            if (t == "(")
            {
                long v = ParseSum(e, ref pos);
                pos++;
                return v;
            }

            if (t == "-")
            {
                return -ParseAtom(e, ref pos);
            }

            if (TryParseInteger(t, out long n))
            {
                return n;
            }

            if (engine._constants.TryGetValue(t, out long c))
            {
                return c;
            }

            throw new FormatException($"Unknown constant '{t}' in array size or enum value.");
        }

        private void SkipTrailingDeclarators()
        {
            while (_i < tokens.Count && Peek() != ";")
            {
                _i++;
            }

            _i++;
        }

        private void SkipInitializer()
        {
            int depth = 0;
            while (_i < tokens.Count)
            {
                string t = Peek();
                if (t is "(" or "{" or "[")
                {
                    depth++;
                }
                else if (t is ")" or "}" or "]")
                {
                    depth--;
                }
                else if (depth == 0 && t is "," or ";")
                {
                    return;
                }

                _i++;
            }
        }

        // Skips a declaration or definition: up to ';' at depth 0, or through a '{...}' body at paren depth 0.
        private void SkipDeclaration()
        {
            int paren = 0;
            while (_i < tokens.Count)
            {
                if (TryDirective())
                {
                    continue;
                }

                string t = Peek();
                if (t is "(" or "[")
                {
                    paren++;
                }
                else if (t is ")" or "]")
                {
                    paren--;
                }
                else if (paren == 0 && t == ";")
                {
                    _i++;
                    return;
                }
                else if (paren == 0 && t == "{")
                {
                    SkipBlock();
                    if (Peek() == ";")
                    {
                        _i++;
                    }

                    return;
                }
                else if (paren == 0 && t == "}")
                {
                    return;
                }

                _i++;
            }
        }

        private void SkipBlock()
        {
            int depth = 0;
            while (_i < tokens.Count)
            {
                string t = Peek();
                _i++;
                if (t == "{")
                {
                    depth++;
                }
                else if (t == "}" && --depth == 0)
                {
                    return;
                }
            }
        }

        private void Expect(string token)
        {
            if (Peek() != token)
            {
                throw new FormatException($"Expected '{token}' but found '{Peek()}' (token {_i}).");
            }

            _i++;
        }

        private static bool IsIdentifier(string t) =>
            t.Length > 0 && (char.IsLetter(t[0]) || t[0] == '_') && !t.StartsWith("#pack:", StringComparison.Ordinal);
    }
}

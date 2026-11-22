using SimApex.Sim.Lmu.Sdk;

namespace SimApex.Tests;

// Expected offsets are worked out by hand from the Microsoft x64 rules:
// alignment = min(natural alignment, #pragma pack), default pack 8, long = 4 bytes, pointers = 8.
public class CppLayoutTests
{
    private const string Header = """
        #pragma once
        #define MY_LEN (5)
        // comment with a { brace and a ; semicolon
        #pragma pack( push, 4 )
        enum class Small : uint8_t { A = 0x00, B, C = 0X10 };
        enum Ev : uint32_t { E0, E1, E2, E_MAX };
        struct V3
        {
            union
            {
                struct { double x, y, z; };
                double data[3];
            };
            void Set( const double a ) { x = a; }
            double &operator[]( long i ) { return( data[ i ] ); }
        };
        struct P4
        {
          long id;                 /* 0 */
          double t;                // 4 (pack 4)
          char name[MY_LEN];       // 12
          Small cls;               // 17
          V3 pos;                  // 20
          unsigned long long sid;  // 44
          float f;                 // 52
        };
        #pragma pack( pop )
        struct D8 { long id; double t; };
        struct Holder
        {
          Ev events[Ev::E_MAX];
          long ver;
          P4 p;
          size_t n;
          D8 d[2];
          char *s;
          bool b;
        };
        class Poly
        {
        public:
          virtual ~Poly() {}
          int a = 1;
          Poly() : a(1) {}
          static int F( int x ) { return x; }
        private:
          struct Inner { volatile LONG w; volatile LONG busy; };
          HANDLE h = NULL;
        };
        static void Fn( Holder &h ) { if( h.b ) { h.n = 0; } }
        """;

    private static CppLayoutEngine Engine()
    {
        var engine = new CppLayoutEngine();
        engine.AddHeader(Header);
        return engine;
    }

    [Fact]
    public void Pack4_places_double_after_long_at_offset_4()
    {
        var p4 = Engine().GetLayout("P4");

        Assert.Equal(4, p4.Find("t")!.Offset);
        Assert.Equal(12, p4.Find("name")!.Offset);
        Assert.Equal(17, p4.Find("cls")!.Offset);
        Assert.Equal(PrimitiveKind.UInt8, p4.Find("cls")!.Kind);
        Assert.Equal(20, p4.Find("pos")!.Offset);
        Assert.Equal(44, p4.Find("sid")!.Offset);
        Assert.Equal(52, p4.Find("f")!.Offset);
        Assert.Equal(56, p4.Size);
        Assert.Equal(4, p4.Alignment);
    }

    [Fact]
    public void Anonymous_union_and_struct_are_flattened()
    {
        var v3 = Engine().GetLayout("V3");

        Assert.Equal(24, v3.Size);
        Assert.Equal(0, v3.Find("x")!.Offset);
        Assert.Equal(16, v3.Find("z")!.Offset);
        Assert.Equal(0, v3.Find("data")!.Offset);
        Assert.Equal([3], v3.Find("data")!.Dimensions);
    }

    [Fact]
    public void Default_pack_aligns_size_t_to_8_after_packed_struct()
    {
        var engine = Engine();
        var holder = engine.GetLayout("Holder");

        Assert.Equal(3, engine.GetConstant("Ev::E_MAX"));
        Assert.Equal(12, holder.Find("ver")!.Offset);
        Assert.Equal(16, holder.Find("p")!.Offset);
        Assert.Equal(72, holder.Find("n")!.Offset);
        Assert.Equal(80, holder.Find("d")!.Offset);
        Assert.Equal(112, holder.Find("s")!.Offset);
        Assert.Equal(PrimitiveKind.Pointer, holder.Find("s")!.Kind);
        Assert.Equal(120, holder.Find("b")!.Offset);
        Assert.Equal(128, holder.Size);
        Assert.Equal(16, engine.GetLayout("D8").Size);
    }

    [Fact]
    public void Virtual_class_gets_vtable_pointer_and_functions_take_no_space()
    {
        var engine = Engine();
        var poly = engine.GetLayout("Poly");

        Assert.Equal(8, poly.Find("a")!.Offset);
        Assert.Equal(16, poly.Find("h")!.Offset);
        Assert.Equal(24, poly.Size);
        Assert.Equal(8, engine.GetLayout("Inner").Size);
        Assert.Equal(16, engine.GetConstant("C"));
        Assert.Equal(260, engine.GetConstant("MAX_PATH"));
    }

    [Fact]
    public void Unknown_type_is_rejected_instead_of_guessed()
    {
        var engine = new CppLayoutEngine();
        engine.AddHeader("struct S { Mystery m; };");

        Assert.Throws<FormatException>(() => engine.GetLayout("S"));
    }
}

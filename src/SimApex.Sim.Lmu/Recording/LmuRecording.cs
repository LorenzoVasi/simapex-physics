using System.Globalization;

namespace SimApex.Sim.Lmu.Recording;

/// <summary>
/// Reads a session written by <c>SimApex.Diagnostics record</c>: <c>lmu_base.bin</c> (first full copy of
/// the mapping), <c>lmu_frames.bin</c> (per tick: int64 timestamp, int32 run count, then runs of
/// int32 offset, int32 length, bytes) and <c>session.txt</c> (timestamp frequency).
/// </summary>
public static class LmuRecording
{
    /// <summary>Ticks per second of the recorded timestamps.</summary>
    public static long TimestampFrequency(string folder)
    {
        foreach (string line in File.ReadLines(Path.Combine(folder, "session.txt")))
        {
            if (line.StartsWith("timestamp_frequency ", StringComparison.Ordinal))
            {
                return long.Parse(line["timestamp_frequency ".Length..], CultureInfo.InvariantCulture);
            }
        }

        throw new InvalidDataException("session.txt has no timestamp_frequency line.");
    }

    /// <summary>
    /// Replays the recording. The same buffer is updated in place and yielded for every frame, so copy it
    /// if a frame must be kept.
    /// </summary>
    public static IEnumerable<(long Timestamp, byte[] Memory)> ReadFrames(string folder)
    {
        byte[] memory = File.ReadAllBytes(Path.Combine(folder, "lmu_base.bin"));
        using var reader = new BinaryReader(File.OpenRead(Path.Combine(folder, "lmu_frames.bin")));
        long length = reader.BaseStream.Length;
        while (reader.BaseStream.Position < length)
        {
            long timestamp = reader.ReadInt64();
            int runs = reader.ReadInt32();
            for (int r = 0; r < runs; r++)
            {
                int offset = reader.ReadInt32();
                int count = reader.ReadInt32();
                if (offset < 0 || count < 0 || offset + count > memory.Length)
                {
                    throw new InvalidDataException($"Corrupt run at stream position {reader.BaseStream.Position}.");
                }

                int read = reader.Read(memory, offset, count);
                if (read != count)
                {
                    yield break; // truncated last frame (recorder stopped mid-write)
                }
            }

            yield return (timestamp, memory);
        }
    }
}

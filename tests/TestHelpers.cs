using System.Text.Json;
using FluentAssertions;
using JPoke;

namespace JPokeTests;

public class TestHelpers
{
    protected static void Check(JObjectBuilder b, object expected)
    {
        var e = JsonSerializer.Serialize(expected, new
            JsonSerializerOptions
            {
                WriteIndented = true
            });
        Console.WriteLine($"expected:{e}");
        var actual = b.Serialize();
        Console.WriteLine($"  actual:{actual}");
        actual
            .Should().Be(e);
    }
}
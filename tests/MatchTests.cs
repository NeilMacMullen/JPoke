using FluentAssertions;
using JPoke;

namespace JPokeTests;

[TestClass]
public class MatchTests
{
    [TestMethod]
    public void SimpleTest()
    {
        var builder = JObjectBuilder.FromObject(
            new {A=123}
            );

        builder.Match("A")
            .Result.Should().Be(JObjectBuilder.MatchType.Object);

        builder.Match("B")
            .Result.Should().Be(JObjectBuilder.MatchType.MissingObjectProperty);

        builder.Match("A[0]")
            .Result.Should().Be(JObjectBuilder.MatchType.IncompatibleType);

        //incompatible type because we already defined A as a value "123"
        builder.Match("A.B")
            .Result.Should().Be(JObjectBuilder.MatchType.IncompatibleType);
    }

    [TestMethod]
    public void ArrayTest()
    {
        var builder = JObjectBuilder.FromObject(
            new { A = new[] {1,2}  }
        );

        builder.Match("A")
            .Result.Should().Be(JObjectBuilder.MatchType.Object);

        builder.Match("A[0]")
            .Result.Should().Be(JObjectBuilder.MatchType.Object);

        builder.Match("A[10]")
            .Result.Should().Be(JObjectBuilder.MatchType.IncompleteArray);

    }
}
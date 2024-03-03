using System.Text.Json.Nodes;
using FluentAssertions;
using JPoke;
using MatchType = JPoke.MatchType;

namespace JPokeTests;

[TestClass]
public class SearchTests
{
    [TestMethod]
    public void SearchEmpty()
    {
        var builder = JObjectBuilder.FromObject(
            new { A = 123 }
        );

        var successfulSearch = builder.Search("");
        successfulSearch.Match.Should().Be(MatchType.Object);

        successfulSearch.ResultOrParent.Should().Be(builder);
    }

    [TestMethod]
    public void SimplePropertySearch()
    {
        var builder = JObjectBuilder.FromObject(
            new { A = 123 }
        );

        var successfulSearch = builder.Search("A");
        successfulSearch.Match.Should().Be(MatchType.Object);

        successfulSearch.ResultOrParent
            .GetAsValue<int>()
            .Should().Be(123);

        var failingSearch = builder.Search("B");
        failingSearch.Match.Should().Be(MatchType.MissingObjectProperty);
        failingSearch.ResultOrParent.Should().Be(builder);
        failingSearch.Remaining.Top.Name.Should().Be("B");
    }

    [TestMethod]
    public void SimpleArraySearch()
    {
        var builder = JObjectBuilder.FromObject(
            new { A = new[] { 1, 2, 3 } }
        );

        var successfulSearch = builder.Search("A");
        successfulSearch.Match.Should().Be(MatchType.Object);

        successfulSearch
            .ResultOrParent
            .ToJsonNode()
            .Should().BeOfType<JsonArray>();

        var successfulIndexSearch = builder.Search("A[1]");
        successfulIndexSearch.Match.Should().Be(MatchType.Object);
        successfulIndexSearch.ResultOrParent.GetAsValue<int>().Should().Be(2);


        var failingSearch = builder.Search("A[5]");
        failingSearch.Match.Should().Be(MatchType.IncompleteArray);
        failingSearch.ResultOrParent.ToJsonNode().Should().BeOfType<JsonArray>();
        failingSearch.Remaining.Top.Name.Should().Be("A");
    }
}
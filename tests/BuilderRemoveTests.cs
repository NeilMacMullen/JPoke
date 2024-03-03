using JPoke;

namespace JPokeTests;

[TestClass]
public class BuilderRemoveTests : TestHelpers
{
    [TestMethod]
    public void NodeCanBeRemoved()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", 123);
        b.Remove("abc.def");
        var expected = new
        {
            abc = new { }
        };
        Check(b, expected);
    }
}

[TestClass]
public class BuilderCopyMoveTests : TestHelpers
{
    [TestMethod]
    public void NodeCanBeRemoved()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", 123);
        b.Move("abc.def", "x.y");
        var expected = new
        {
            abc = new { },
            x = new { y = 123 }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void NodeCanBeCopied()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", 123);
        b.Copy("abc.def", "x.y");
        var expected = new
        {
            abc = new { def = 123 },
            x = new { y = 123 }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void NodeCanBeMoved()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", 123);
        b.Move("abc.def", "x.y");
        var expected = new
        {
            abc = new object(),
            x = new { y = 123 }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void BuilderCanBeInserted()
    {
        var top = JObjectBuilder.CreateEmpty();
        var child = JObjectBuilder.CreateEmpty();
        child.Set("def", 123);

        top.Set("abc", child);

        var expected = new
        {
            abc = new { def = 123 }
        };
        Check(top, expected);
    }
}
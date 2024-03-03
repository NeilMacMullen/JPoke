using JPoke;

namespace JPokeTests;

[TestClass]
public class BuilderSetTests : TestHelpers
{
    [TestMethod]
    public void RootCanBeSetToObject()
    {
        var b = JObjectBuilder.CreateEmpty();
        var o = new { abc = 123 };
        b.Set("", o);
        var expected = o;
        Check(b, expected);
    }

    [TestMethod]
    public void RootCanBeSetToValue()
    {
        var b = JObjectBuilder.CreateEmpty();
        var o = 123;
        b.Set("", o);
        var expected = o;
        Check(b, expected);
    }

    [TestMethod]
    public void RootCanBeSetToArray()
    {
        var b = JObjectBuilder.CreateEmpty();
        var o = new[] { 123 };
        b.Set("", o);
        var expected = o;
        Check(b, expected);
    }


    [TestMethod]
    public void RootArrayCanBeSet()
    {
        var b = JObjectBuilder.CreateEmpty();
        var o = 123;
        b.Set("[0]", o);
        var expected = new[] { o };
        Check(b, expected);
    }


    [TestMethod]
    public void SimpleElementIsParsed()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", 123);
        var expected = new
        {
            abc = new
            {
                def = 123
            }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void ElementCanBeAdded()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", 123);
        b.Set("abc.xyz", 456);
        var expected = new
        {
            abc = new
            {
                def = 123,
                xyz = 456
            }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void ArrayElementCanBeSet()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def[0]", 123);

        var expected = new
        {
            abc = new
            {
                def = new[] { 123 }
            }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void ObjectCanBeSet()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def", new { xyz = 5 });

        var expected = new
        {
            abc = new
            {
                def = new { xyz = 5 }
            }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void ArrayCanBeAdded()
    {
        var b = JObjectBuilder.CreateEmpty();
        var arr = new[] { "first", "second" };
        b.Set("abc.def", arr);

        var expected = new
        {
            abc = new
            {
                def = arr
            }
        };
        Check(b, expected);
    }


    [TestMethod]
    public void ArrayElementCanBeSetInPassing()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def[0].a", 123);
        var expected = new
        {
            abc = new
            {
                def = new[]
                {
                    new
                    {
                        a = 123
                    }
                }
            }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void ArrayElementCanBeSetUsingIncrementalSyntax()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def[+0]", 123);

        var expected = new
        {
            abc = new
            {
                def = new int?[] { 123 }
            }
        };
        Check(b, expected);
    }

    [TestMethod]
    public void MultipleArrayElementsCanBeSetUsingIncrementalSyntax()
    {
        var b = JObjectBuilder.CreateEmpty();
        b.Set("abc.def[+0]", 123);
        b.Set("abc.def[+0]", 456);

        var expected = new
        {
            abc = new
            {
                def = new int?[] { 123, 456 }
            }
        };
        Check(b, expected);
    }
}
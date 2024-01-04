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
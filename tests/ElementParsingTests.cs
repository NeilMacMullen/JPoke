using FluentAssertions;
using JPoke;

namespace JPokeTests
{
    [TestClass]
    public class ElementParsingTests
    {
        [TestMethod]
        public void SimpleElementIsParsed()
        {
            var e = PathParser.ParseElement("abc");
            e.Name.Should().Be("abc");
            e.IsIndex.Should().BeFalse();
        }

        [TestMethod]
        public void IndexedElementIsParsed()
        {
            var e = PathParser.ParseElement("abc[1]");
            e.Name.Should().Be("abc");
            e.IsIndex.Should().BeTrue();
            e.Index.RawValue.Should().Be(1);
        }

        [TestMethod]
        public void EmptyIndexerReturnsRelativeIndexOfMinus1()
        {
            var e = PathParser.ParseElement("abc[]");
            e.Name.Should().Be("abc");
            e.IsIndex.Should().BeTrue();
            e.Index.RawValue.Should().Be(-1);
            e.Index.IndexType.Should().Be(JPathIndexType.Relative);
        }
    }
}
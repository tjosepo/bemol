using System.IO;
using Xunit;
using Bemol.Core;

namespace Bemol.Test {
    public class BemolRendererTest {

        readonly BemolRenderer Renderer = new BemolRenderer(new BemolConfig());

        [Theory]
        [InlineData("foo", "foo")]
        [InlineData("/foo", "foo")]
        [InlineData("foo/", "foo")]
        [InlineData("/foo/", "foo")]
        [InlineData("\\foo/", "foo")]
        [InlineData("/foo\\", "foo")]
        [InlineData("\\foo.liquid", "foo.liquid")]
        [InlineData("/foo/bar.liquid", "foo/bar.liquid")]
        [InlineData("\\foo\\bar.liquid", "foo/bar.liquid")]
        public void NormalizePaths_Equal(string input, string expected) {
            var separator = Path.DirectorySeparatorChar;
            expected = expected.Replace("/", $"{separator}");

            var result = Renderer.NormalizePaths(input);
            Assert.Equal(result, expected);
        }
    }
}
using System.IO;

using Xunit;
using Bemol.Core;

namespace Bemol.Test {
    public class BemolUtilTest {

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
        public void NormalizePath_Equal(string input, string expected) {
            var separator = Path.DirectorySeparatorChar;
            expected = expected.Replace("/", $"{separator}");

            var result = BemolUtil.NormalizePath(input);
            Assert.Equal(result, expected);
        }
    }
}
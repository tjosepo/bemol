using Xunit;
using Bemol.Core;

namespace BemolTest {
    public class ContextPathTest {

        [Fact]
        public void Matches_Normal_FindsMatch() {
            var parser = new PathParser("/foo", true);
            Assert.True(parser.Matches("/foo"));
        }

        [Fact]
        public void Matches_Normal_NoMatch() {
            var parser = new PathParser("/foo", true);
            Assert.False(parser.Matches("/bar"));
        }

        [Fact]
        public void Matches_IgnoneTrailingSlash_FindsMatch() {
            var parser = new PathParser("/foo/", true);
            Assert.True(parser.Matches("/foo"));

            parser = new PathParser("/foo", true);
            Assert.True(parser.Matches("/foo/"));
        }

        [Fact]
        public void Matches_EnforceTrailingSlash_FindsMatch() {
            var parser = new PathParser("/foo/", false);
            Assert.True(parser.Matches("/foo/"));
        }

        [Fact]
        public void Matches_EnforceTrailingSlash_NoMatch() {
            var parser = new PathParser("/foo/", false);
            Assert.False(parser.Matches("/foo"));
        }

        [Fact]
        public void Matches_Parameter_FindsMatch() {
            var parser = new PathParser("/foo/:param", false);
            Assert.True(parser.Matches("/foo/bar"));
            Assert.True(parser.Matches("/foo/baz"));
            Assert.True(parser.Matches("/foo/quux"));
            Assert.True(parser.Matches("/foo/corge"));
        }

        [Fact]
        public void Matches_Parameter_NoMatch() {
            var parser = new PathParser("/foo/:param", false);
            Assert.False(parser.Matches("/foo/bar/baz"));
        }

        [Fact]
        public void Matches_Wildcard_FindsMatch() {
            var parser = new PathParser("/*", false);
            Assert.True(parser.Matches("/foo"));
            Assert.True(parser.Matches("/bar"));
            Assert.True(parser.Matches("/baz"));
            Assert.True(parser.Matches("/foo/bar"));
            Assert.True(parser.Matches("/foo/baz"));
            Assert.True(parser.Matches("/foo/bar/baz"));
        }

        [Fact]
        public void Matches_Wildcard_NoMatch() {
            var parser = new PathParser("/foo/*", false);
            Assert.False(parser.Matches("/foo"));
        }

        [Theory]
        [InlineData("/:foo", "foo", "/bar", "bar")]
        [InlineData("/:foo/bar", "foo", "/baz/bar", "baz")]
        [InlineData("/foo/:bar", "bar", "/foo/baz", "baz")]
        [InlineData("/*/:foo", "foo", "/foobarbaz/baz", "baz")]
        public void ExtractPathParams_FindsMatch(string path, string key, string url, string expected) {
            var parser = new PathParser(path, false);
            var pathParamDict = parser.ExtractPathParams(url);
            Assert.Equal(expected, pathParamDict[key]);
        }

        [Fact]
        public void ExtractPathParams_NoMatch() {
            var parser = new PathParser("/:foo", false);
            var pathParamDict = parser.ExtractPathParams("/bar");
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => pathParamDict["baz"]);
        }
    }
}
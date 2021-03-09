using Xunit;
using static Bemol.Routing.PathParser;

namespace Bemol.Test.Routing {
  public class PathParserTest {

    [Fact]
    public void Matches_Normal_FindsMatch() {
      Assert.True(Matches("/foo", "/foo"));
    }

    [Fact]
    public void Matches_Normal_NoMatch() {
      Assert.False(Matches("/foo", "/bar"));
    }

    [Fact]
    public void Matches_IgnoneTrailingSlash_FindsMatch() {
      Assert.True(Matches("/foo", "/foo/", true));
      Assert.True(Matches("/foo/", "/foo", true));
    }

    [Fact]
    public void Matches_EnforceTrailingSlash_FindsMatch() {
      Assert.True(Matches("/foo/", "/foo/", false));
    }

    [Fact]
    public void Matches_EnforceTrailingSlash_NoMatch() {
      Assert.False(Matches("/foo/", "/foo", false));
      Assert.False(Matches("/foo", "/foo/", false));
    }

    [Fact]
    public void Matches_Parameter_FindsMatch() {
      Assert.True(Matches("/foo/:param", "/foo/bar"));
      Assert.True(Matches("/foo/:param", "/foo/baz"));
      Assert.True(Matches("/foo/:param", "/foo/quux"));
      Assert.True(Matches("/foo/:param", "/foo/corge"));
    }

    [Fact]
    public void Matches_Parameter_NoMatch() {
      Assert.False(Matches("/foo/:param", "/foo/bar/baz"));
    }

    [Fact]
    public void Matches_Wildcard_FindsMatch() {
      Assert.True(Matches("*", "/foo"));
      Assert.True(Matches("*", "/bar"));
      Assert.True(Matches("*", "/baz"));
      Assert.True(Matches("*", "/foo/bar"));
      Assert.True(Matches("*", "/foo/baz"));
      Assert.True(Matches("*", "/foo/bar/baz"));
    }

    [Fact]
    public void Matches_Wildcard_NoMatch() {
      Assert.False(Matches("/foo/*", "/foo"));
    }

    [Theory]
    [InlineData("/:foo", "foo", "/bar", "bar")]
    [InlineData("/:foo/bar", "foo", "/baz/bar", "baz")]
    [InlineData("/foo/:bar", "bar", "/foo/baz", "baz")]
    [InlineData("/*/:foo", "foo", "/foobarbaz/baz", "baz")]
    public void ExtractPathParams_FindsMatch(string route, string key, string url, string expected) {
      var pathParamDict = ExtractPathParams(route, url);
      Assert.Equal(expected, pathParamDict[key]);
    }

    [Fact]
    public void ExtractPathParams_NoMatch() {
      var pathParamDict = ExtractPathParams("/:foo", "/bar");
      Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => pathParamDict["baz"]);
    }

    [Fact]
    public void ExtractPathParams_RoutesDontMatch() {
      var pathParamDict = ExtractPathParams("/foo/:bar", "/baz/baz");
      Assert.Equal("", pathParamDict["bar"]);
    }
  }
}
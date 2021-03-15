using Xunit;
using Bemol.Routing;

namespace Bemol.Test {
  public class RouterTest {

    [Theory]
    [InlineData("get", "/", "/")]
    [InlineData("Get", "/", "/")]
    [InlineData("GET", "/", "/")]
    [InlineData("Post", "", "/")]
    [InlineData("GET", "foo", "/foo")]
    [InlineData("Post", "/foo", "/foo")]
    [InlineData("Post", "//foo", "/foo")]
    [InlineData("Post", "/foo/bar", "/foo/bar")]
    [InlineData("Post", "///foo///bar", "/foo/bar")]
    [InlineData("Put", "//:name", "/foo")]
    [InlineData("Put", "//:name//bar", "/foo/bar")]
    [InlineData("Put", "*", "/foo/bar")]
    public void Add_Route_Find(string method, string path, string url) {
      var router = new Router();
      var route = new Route(method, path, null, null);
      router.Add(route);
      var result = router.Find(method, url);
      Assert.Single(result);
    }

    [Fact]
    public void Add_Router_Find() {
      var routerA = new Router();
      var routerB = new Router();
      routerB.Get("/hello", (ctx) => { });
      routerA.Add(routerB);
      var result = routerA.Find("Get", "/hello");
      Assert.Single(result);
    }

    [Fact]
    public void Add_RouterPrefix_Find() {
      var routerA = new Router();
      var routerB = new Router();
      var routerC = new Router();

      routerA.Get("/foo", (ctx) => { });
      var result = routerA.Find("Get", "/foo");
      Assert.Single(result);

      routerB.Add("/bar", routerA);
      result = routerB.Find("Get", "/bar/foo");
      Assert.Single(result);

      routerC.Add("/baz", routerB);
      result = routerC.Find("Get", "/baz/bar/foo");
      Assert.Single(result);
    }

    [Theory]
    [InlineData("/", "/")]
    [InlineData("", "/")]
    [InlineData("foo", "/foo")]
    [InlineData("/foo", "/foo")]
    [InlineData("//foo", "/foo")]
    [InlineData("/foo/bar", "/foo/bar")]
    [InlineData("///foo///bar", "/foo/bar")]
    [InlineData("//:name", "/foo")]
    [InlineData("//:name//bar", "/foo/bar")]
    [InlineData("*", "/foo/bar")]
    public void Get_Find(string path, string url) {
      var router = new Router();
      router.Get(path, (ctx) => { });
      var result = router.Find("Get", url);
      Assert.Single(result);
    }

    [Theory]
    [InlineData("/", "/")]
    [InlineData("", "/")]
    [InlineData("foo", "/foo")]
    [InlineData("/foo", "/foo")]
    [InlineData("//foo", "/foo")]
    [InlineData("/foo/bar", "/foo/bar")]
    [InlineData("///foo///bar", "/foo/bar")]
    [InlineData("//:name", "/foo")]
    [InlineData("//:name//bar", "/foo/bar")]
    [InlineData("*", "/foo/bar")]
    public void Add_Find(string path, string url) {
      var router = new Router();
      router.Add("Get", path, (ctx) => { });
      var result = router.Find("Get", url);
      Assert.Single(result);
    }
  }
}
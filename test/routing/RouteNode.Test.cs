using Xunit;
using System.Linq;

using Bemol.Routing;

namespace Bemol.Test {
  public class RouteNodeTest {

    [Theory]
    [InlineData("/hello/world")]
    [InlineData("*/why")]
    public void Add_Route_Single(string path) {
      var route = new Bemol.Routing.Route("GET", path);

      var routeNode = new RouteNode();
      routeNode.Add(path, route);
      var endpoints = routeNode.Get(path);
      Assert.Equal(route, endpoints.Single());
    }

    [Theory]
    [InlineData(":id", "/foo")]
    [InlineData("/foo/:id", "/foo/bar")]
    [InlineData("/:id/bar", "/foo/bar")]
    public void Add_Parameter(string path, string equivalentPath) {
      var routeNode = new RouteNode();

      var parameter = new Bemol.Routing.Route("GET", path);
      routeNode.Add(path, parameter);

      var endpoints = routeNode.Get(equivalentPath);
      Assert.Equal(parameter, endpoints.Single());
    }

    [Theory]
    [InlineData("*", "/foo")]
    [InlineData("/foo/*", "/foo/bar")]
    [InlineData("/*/bar", "/foo/bar")]
    public void Add_Wildcards(string path, string equivalentPath) {
      var routeNode = new RouteNode();

      var wildcard = new Bemol.Routing.Route("GET", path);
      routeNode.Add(path, wildcard);

      var endpoints = routeNode.Get(equivalentPath);
      Assert.Equal(wildcard, endpoints.Single());
    }

    [Fact]
    public void Add_Wildcards_Many() {
      var routeNode = new RouteNode();

      var wildcard = new Bemol.Routing.Route("GET", "*");
      routeNode.Add(wildcard.Path, wildcard);

      var route = new Bemol.Routing.Route("GET", "/hello/world");
      routeNode.Add(route.Path, route);

      var endpoints = routeNode.Get("/hello/world");
      Assert.Equal(wildcard, endpoints[0]);
      Assert.Equal(route, endpoints[1]);
    }

    [Fact]
    public void Get_Route_Invalid() {
      var routeNode = new RouteNode();
      var endpoints = routeNode.Get("/foo/bar");

      Assert.Empty(endpoints);
    }
  }
}
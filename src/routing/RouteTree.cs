using System.Linq;
using System.Collections.Generic;

namespace Bemol.Routing {
  internal interface IRouteNode { };

  public class RouteNode : IRouteNode {
    private Dictionary<string, RouteNode> Childrens = new Dictionary<string, RouteNode>();
    private List<Routing.Route> Endpoints = new List<Routing.Route>();
    private List<(string, Routing.Route)> PartialEndpoints = new List<(string, Routing.Route)>();

    internal void Add(string? path, Routing.Route route) {
      var (segment, remainder) = PathParser.Cons(path);

      if (segment is null) Endpoints.Add(route);
      else if (segment.StartsWith(":")) PartialEndpoints.Add((path!, route));
      else if (segment == "*") PartialEndpoints.Add((path!, route));
      else {
        Childrens.TryAdd(segment, new RouteNode());
        var child = Childrens[segment];
        child.Add(remainder, route);
      }
    }

    internal List<Route> Get(string? path) {
      var (segment, remainder) = PathParser.Cons(path);

      if (segment is null) {
        return Endpoints;
      }

      var children = Childrens.GetValueOrDefault(segment, new RouteNode());
      return new List<Route>()
        .Concat(GetPartialEndpoints(path))
        .Concat(children.Get(remainder))
        .ToList();

    }

    private List<Route> GetPartialEndpoints(string? path) {
      return PartialEndpoints.Where((endpoint) => {
        var (endpointPath, _) = endpoint;
        return PathParser.Matches(endpointPath ?? "", path);
      }).Select(endpoint => {
        var (path, route) = endpoint;
        return route;
      }).ToList();
    }
  }
}
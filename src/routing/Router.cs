using System.Linq;
using System.Collections.Generic;

using Bemol.Http;

namespace Bemol.Routing {
  /// <summary> Represents a collection of Routes. </summary>
  public class Router {
    internal readonly Dictionary<string, List<Route>> RouteMap = new Dictionary<string, List<Route>>();

    /// <summary> Adds a custom request route to the instance. </summary>
    internal Router Add(Route route) {
      RouteMap.TryAdd(route.Method, new List<Route>());
      var methods = RouteMap[route.Method];
      methods.Add(route);
      return this;
    }

    /// <summary> Adds a custom router to the instance. </summary>
    public Router Add(Router router) => Add("", router);

    /// <summary> Adds a custom router to the instance. </summary>
    public Router Add(string path, Router router) {
      foreach (var item in router.RouteMap) {
        RouteMap.TryAdd(item.Key, new List<Route>());
        var methods = RouteMap[item.Key];
        foreach (var route in item.Value) {
          methods.Add(new Route {
            Method = route.Method,
            Path = path + route.Path,
            Handler = route.Handler
          });
        }
      }
      return this;
    }

    /// <summary> Find the list of Routes that match the method and path. </summary>
    internal List<Route> Find(string method, string path) {
      return RouteMap.GetValueOrDefault(method.ToUpper(), new List<Route>())
          .Where(route => PathParser.Matches(route.Path, path))
          .ToList();
    }

    // ********************************************************************************************
    // HTTP
    // ********************************************************************************************

    /// <summary> Adds a custom request handler for the specified method and path to the router. </summary>
    public Router Add(string method, string path, Handler handler) {
      return Add(new Route {
        Method = method.ToUpper(),
        Path = path,
        Handler = handler
      });
    }

    /// <summary> Adds a GET request handler for the specified path to the router. </summary>
    public Router Get(string path, Handler handler) => Add("GET", path, handler);

    /// <summary> Adds a POST request handler for the specified path to the router. </summary>
    public Router Post(string path, Handler handler) => Add("POST", path, handler);

    /// <summary> Adds a PUT request handler for the specified path to the router. </summary>
    public Router Put(string path, Handler handler) => Add("PUT", path, handler);

    /// <summary> Adds a DELETE request handler for the specified path to the router. </summary>
    public Router Delete(string path, Handler handler) => Add("DELETE", path, handler);
  }
}
using System;
using System.Linq;
using System.Collections.Generic;

using Bemol.Http;
using Bemol.Routing;

namespace Bemol {
  /// <summary> Represents a collection of Routes. </summary>
  public class Router {
    private readonly Dictionary<string, List<Route>> RouteMap = new Dictionary<string, List<Route>>();
    private readonly List<Middleware> Middlewares = new List<Middleware>();
    private readonly RouterConfig Config = new RouterConfig();

    public Router() { }
    public Router(Action<RouterConfig> config) : base() => config.Invoke(Config);

    /// <summary> Adds a custom request route to the instance. </summary>
    public Router Add(Route route) {
      Middlewares.ForEach(route.Middlewares.Add);
      RouteMap.TryAdd(route.Method, new List<Route>());
      var methods = RouteMap[route.Method];
      methods.Add(route);
      return this;
    }

    /// <summary> Adds a custom middleware to the instance. </summary>
    public Router Add(Handler handler) => Add(new Middleware(handler));

    /// <summary> Adds a custom middleware to the instance. </summary>
    public Router Add(Middleware handler) {
      foreach (var method in RouteMap) {
        var routes = method.Value;
        routes.ForEach(route => route.Middlewares.Add(handler));
      }
      Middlewares.Add(handler);
      return this;
    }

    /// <summary> Adds a custom router to the instance. </summary>
    public Router Add(Router router) => Add("", router);

    /// <summary> Adds a custom router to the instance. </summary>
    public Router Add(string path, Router router) {
      foreach (var method in router.RouteMap) {
        var routes = method.Value;
        routes.ForEach(route => Add(new Route {
          Method = route.Method,
          Path = path + '/' + route.Path,
          Handler = route.Handler,
          Config = route.Config,
          Middlewares = route.Middlewares.Union(this.Middlewares).ToList()
        }));
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
      return Add(new Route(method, path, handler, Config));
    }

    /// <summary> Adds a BEFORE request handler for the specified path to the router. </summary>
    public Router Before(Handler handler) => Before("*", handler);

    /// <summary> Adds a BEFORE request handler for the specified path to the router. </summary>
    public Router Before(string path, Handler handler) => Add("BEFORE", path, handler);

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
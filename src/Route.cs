using System.Collections.Generic;

using Bemol.Http;
using Bemol.Routing;

namespace Bemol {
  /// <summary> A base class to identify and handle a request to a route. </summary>
  public class Route {
    private string _Method;
    public string Method {
      get => _Method;
      set => _Method = value.ToUpper();
    }
    public string Path;
    public Handler Handler;
    public RouterConfig Config;
    public List<Middleware> Middlewares = new List<Middleware>();

    public Route() { }

    public Route(string method, string path, Handler handler, RouterConfig config) {
      _Method = method.ToUpper();
      Path = path;
      Handler = handler;
      Config = config;
    }

    public Route(string method, string path, Handler handler, RouterConfig config, List<Middleware> middlewares) {
      _Method = method.ToUpper();
      Path = path;
      Handler = handler;
      Config = config;
      Middlewares = middlewares;
    }
  }
}
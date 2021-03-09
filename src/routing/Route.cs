using Bemol;
using Bemol.Http;

namespace Bemol.Routing {
  /// <summary> A base class to identify and handle a request to a route. </summary>
  internal readonly struct Route {
    private readonly string _Method;
    internal string Method {
      get => _Method;
      init => _Method = value.ToUpper();
    }
    internal string Path { get; init; }
    internal Handler Handler { get; init; }
    // public RouteConfig Config { get; init; }
  }
}
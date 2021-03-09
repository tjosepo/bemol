using Bemol;
using Bemol.Http;

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
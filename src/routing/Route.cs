namespace Bemol.Routing {
  public record Route(string Method, string Path) : IRouteNode;
}
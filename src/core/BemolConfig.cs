using Bemol.Routing;

namespace Bemol.Core {
  public class BemolConfig : RouterConfig {
    public string StaticFolder = "/static";
    public string ContextPath = "/";
    public string DefaultContentType = "text/plain";
    public bool EnableCorsForAllOrigins = false;
  }
}
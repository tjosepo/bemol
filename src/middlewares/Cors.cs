using System;

using Bemol.Http;

namespace Bemol.Middlewares {
  public class Cors : Router {
    private CorsConfig Config = new CorsConfig();

    public Cors(Action<CorsConfig> config) : this() => config.Invoke(Config);

    public Cors() {
      Before(SetCorsPolicy);
    }

    private void SetCorsPolicy(Context ctx) {
      ctx.Header("Access-Control-Allow-Origin", Config.AllowOrigins);
    }

    public class CorsConfig {
      /// <summary> Defines a list of origins that may access the resource. </summary>
      public string AllowOrigins = "*";
    }
  }
}
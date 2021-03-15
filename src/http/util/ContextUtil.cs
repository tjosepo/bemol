using System.Web;
using System.Collections.Specialized;

using Bemol.Core;
using Bemol.Routing;

namespace Bemol.Http.Util {
  internal static class ContextUtil {
    internal static Context Update(Context ctx, Route route) {
      ctx.PathParamDict = PathParser.ExtractPathParams(route.Path, ctx.Path());
      ctx.Renderer = new BemolRenderer(route.Config);
      return ctx;
    }

    internal static NameValueCollection SplitKeyValueString(string data) {
      data = data.TrimStart('?');
      var parameters = new NameValueCollection();
      if (data.Length == 0) return parameters;

      var decodedStr = HttpUtility.UrlDecode(data);
      string[] keyValues = decodedStr.Split('&');
      foreach (var keyValue in keyValues) {
        var pair = keyValue.Split('=', 2);
        parameters.Add(pair[0], pair[1]);
      }

      return parameters;
    }
  }
}
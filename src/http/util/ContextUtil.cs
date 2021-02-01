using System.Web;
using System.Collections.Specialized;

using Bemol.Core;

namespace Bemol.Http.Util {
    internal static class ContextUtil {
        internal static Context Update(Context ctx, HandlerEntry entry) {
            ctx.PathParamDict = entry.ExtractPathParams(ctx.Path());
            ctx.Renderer = new BemolRenderer(entry.Config);
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
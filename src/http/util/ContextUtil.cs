using System.Collections.Specialized;
using System.Web;

namespace Bemol.Http.Util {
    public static class ContextUtil {
        public static Context Update(Context ctx, HandlerEntry entry) {
            ctx.PathParamDict = entry.ExtractPathParams(ctx.Path());
            return ctx;
        }

        public static NameValueCollection SplitKeyValueStringAndGroupByKey(string str) {
            var fromData = new NameValueCollection();
            if (str.Length == 0) return fromData;

            var decodedStr = HttpUtility.UrlDecode(str);
            string[] keyValues = decodedStr.Split('&');
            foreach (var keyValue in keyValues) {
                var pair = keyValue.Split('=', 2);
                fromData.Add(pair[0], pair[1]);
            }

            return fromData;
        }
    }
}
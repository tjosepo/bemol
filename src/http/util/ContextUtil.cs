using System.Collections.Generic;
using System.Linq;

namespace Bemol.Http.Util {
    public static class ContextUtil {
        public static Context Update(Context ctx, HandlerEntry entry) {
            ctx.PathParamDict = entry.ExtractPathParams(ctx.Path());
            return ctx;
        }

        public static Dictionary<string, string> SplitKeyValueStringAndGroupByKey(string str) {
            var fromData = new Dictionary<string, string>();
            if (str.Length == 0) return fromData;

            string[] keyValues = str.Split('&');
            foreach (var keyValue in keyValues) {
                var pair = keyValue.Split('=', 2);
                fromData.Add(pair[0], pair[1]);
            }

            return fromData;
        }
    }
}
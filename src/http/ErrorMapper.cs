using System.Collections.Generic;

namespace Bemol.Http {
    internal class ErrorMapper {
        private Dictionary<int, Handler> ErrorHandlerMap = new Dictionary<int, Handler>();

        internal void Handle(int statusCode, Context ctx) {
            if (!ErrorHandlerMap.ContainsKey(statusCode)) return;
            ErrorHandlerMap[statusCode](ctx);
        }

        internal void Add(int statusCode, Handler handler) => ErrorHandlerMap.Add(statusCode, handler);
    }
}
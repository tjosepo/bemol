using System.Collections.Generic;

namespace Bemol.Http {
    public class ErrorMapper {
        private Dictionary<int, Handler> ErrorHandlerMap = new Dictionary<int, Handler>();

        public void Handle(int statusCode, Context ctx) {
            if (!ErrorHandlerMap.ContainsKey(statusCode)) return;
            ErrorHandlerMap[statusCode](ctx);
        }
        public void Add(int statusCode, Handler handler) => ErrorHandlerMap.Add(statusCode, handler);
    }
}
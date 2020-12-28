using System;

using System.Collections.Generic;

namespace Bemol.Http {
    public class ErrorMapper {
        private Dictionary<int, Handler> errorHandlerMap = new Dictionary<int, Handler>();

        public void Handle(int statusCode, Context ctx) {
            if (!errorHandlerMap.ContainsKey(statusCode)) return;
            errorHandlerMap[statusCode](ctx);
        }
        public void Add(int statusCode, Handler handler) => errorHandlerMap.Add(statusCode, handler);
    }
}
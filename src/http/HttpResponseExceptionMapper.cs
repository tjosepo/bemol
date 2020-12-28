using System;

namespace Bemol.Http {
    public class HttpResponseExceptionMapper {
        static public Boolean CanHandle(Exception e) {
            Type t = e.GetType();
            return t.IsAssignableFrom(typeof(HttpResponseException));
        }

        static public void Handle(HttpResponseException e, Context ctx) {
            string result;
            if (ctx.response.ContentType == "application/json") {
                result = $@"{{
                    ""title"": ""{e.message}"",
                    ""status"": {e.status},
                    ""type"": ""{e.GetType().ToString()}"",
                }}";
            } else {
                result = e.message;
            }
            ctx.Status(e.status).Result(result);
        }
    }
}
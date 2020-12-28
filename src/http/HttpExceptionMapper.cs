namespace Bemol.Http {
    public class HttpExceptionMapper {
        static public void Handle(HttpException e, Context ctx) {
        string result;
        if (ctx.Response.ContentType == "application/json") {
            result = $@"{{
                        ""title"": ""{e.Message}"",
                        ""status"": {e.Status},
                        ""type"": ""{e.GetType().ToString()}"",
                    }}";
        } else {
            result = e.Message;
        }
        ctx.Status(e.Status).Result(result);
    }
}
}
using System;
using Bemol.Http.Exceptions;

namespace Bemol.Http {
    public class ExceptionMapper {
        public void CatchException(Context ctx, Action func) {
            try {
                func();
            } catch (HttpException e) {
                Handle(e, ctx);
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                Handle(new InternalServerErrorException(), ctx);
            }
        }

        private void Handle(HttpException e, Context ctx) {
            string result;
            if (ctx.Response.ContentType == "application/json") {
                result = $@"{{""title"": ""{e.Message}"",""status"": {e.Status},""type"": ""{e.GetType().Name}""}}";
            } else {
                result = e.Message;
            }
            ctx.Status(e.Status).Result(result);
        }
    }
}
using System;

namespace Bemol.Http {
    public class ExceptionMapper {
        private void Handle(Exception exception, Context ctx) {
            if (HttpResponseExceptionMapper.CanHandle(exception)) {
                HttpResponseExceptionMapper.Handle((HttpResponseException)exception, ctx);
            } else {
                HttpResponseExceptionMapper.Handle(new InternalServerErrorResponse(), ctx);
            }
        }
        public void CatchException(Context ctx, Action Func) {
            try {
                Func();
            } catch (HttpResponseException e) {
                Console.WriteLine(e.Message);
                HttpResponseExceptionMapper.Handle(e, ctx);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                HttpResponseExceptionMapper.Handle(new InternalServerErrorResponse(), ctx);
            }
        }
    }
}
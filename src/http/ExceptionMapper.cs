using System;

namespace Bemol.Http {
    public class ExceptionMapper {
        public void CatchException(Context ctx, Action func) {
            try {
                func();
            } catch (HttpException e) {
                HttpExceptionMapper.Handle(e, ctx);
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                HttpExceptionMapper.Handle(new InternalServerErrorException(), ctx);
            }
        }
    }
}
namespace Bemol.Http.Util {
    public static class ContextUtil {
        public static Context Update(Context ctx, HandlerEntry entry) {
            ctx.pathParamDict = entry.ExtractPathParams(ctx.Path());
            return ctx;
        }
    }
}
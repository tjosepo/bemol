using System;
using System.Net;
using System.Threading;
using Bemol.Core;
using Bemol.Http.Util;
using Bemol.Http.Exceptions;

namespace Bemol.Http {
    class BemolServer {
        private BemolConfig Config;
        public int Port { set; get; } = 7000;
        public string Host { set; get; } = "localhost";
        public bool Started { set; get; } = false;
        private PathMatcher Matcher = new PathMatcher();
        private ErrorMapper ErrorMapper = new ErrorMapper();
        private ExceptionMapper ExceptionMapper = new ExceptionMapper();

        public BemolServer(BemolConfig config) {
            Config = config;
        }

        public void Start() {
            new Thread(() => {
                var listener = new HttpListener();
                listener.Prefixes.Add($"http://{Host}:{Port}{Config.ContextPath}");
                listener.Start();

                Console.Clear();
                Console.WriteLine($"Listening on http://{Host}:{Port}{Config.ContextPath}");

                while (Started) {
                    var rawCtx = listener.GetContext();
                    var ctx = new Context(rawCtx);

                    ctx.ContentType(Config.DefaultContentType);

                    TryBeforeHandlers(ctx);
                    TryEndpointHandler(ctx);
                    TryErrorHandler(ctx);
                    TryAfterHandlers(ctx);

                    if (Config.EnableCorsForAllOrigins) ctx.Header("Access-Control-Allow-Origin", "*");
                    SendResponse(ctx);
                }
            }).Start();
        }

        public void AddErrorHandler(int statusCode, Handler handler) {
            ErrorMapper.Add(statusCode, handler);
        }

        public void AddHandler(HandlerType type, string path, Handler handler) {
            Matcher.Add(new HandlerEntry(type, path, Config.IgnoreTrailingSlashes, handler));
        }

        private void TryWithExceptionMapper(Context ctx, Action func) {
            ExceptionMapper.CatchException(ctx, func);
        }

        private void TryBeforeHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var beforeEntries = Matcher.FindEntries(HandlerType.BEFORE, ctx.Path());
            beforeEntries.ForEach(entry => {
                entry.Handler(ContextUtil.Update(ctx, entry));
            });
        });

        private void TryErrorHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            ErrorMapper.Handle(ctx.Status(), ctx);
        });

        private void TryEndpointHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var type = Enum.Parse<HandlerType>(ctx.Method());
            var entries = Matcher.FindEntries(type, ctx.Path());
            var entry = entries.FindLast(entry => true);

            if (entry == null) throw new NotFoundException($"'{ctx.Path()}' is not a valid path.");

            entry.Handler(ContextUtil.Update(ctx, entry));
        });

        private void TryAfterHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var afterEntries = Matcher.FindEntries(HandlerType.AFTER, ctx.Path());
            afterEntries.ForEach(entry => {
                entry.Handler(ContextUtil.Update(ctx, entry));
            });
        });

        private void SendResponse(Context ctx) {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ctx.Result());
            ctx.Response.ContentLength64 = buffer.Length;
            System.IO.Stream output = ctx.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            Console.WriteLine($"[{ctx.Method()}] {ctx.Status()} {ctx.Path()}");
        }
    }
}
using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Bemol.Core;
using Bemol.Http.Util;

namespace Bemol.Http {
    class BemolServer {
        private BemolConfig config;
        public int port { set; get; } = 7000;
        public string host { set; get; } = "localhost";
        public bool started { set; get; } = false;
        private PathMatcher matcher = new PathMatcher();
        private ErrorMapper errorMapper = new ErrorMapper();
        private ExceptionMapper exceptionMapper = new ExceptionMapper();
        public BemolServer(BemolConfig config) {
            this.config = config;
        }

        public void Start() {
            new Thread(() => {
                var listener = new HttpListener();
                listener.Prefixes.Add($"http://{host}:{port}{config.contextPath}");
                listener.Start();

                Console.Clear();
                Console.WriteLine($"Listening on http://{host}:{port}{config.contextPath}");

                while (started) {
                    var contextRaw = listener.GetContext();
                    var ctx = new Context(contextRaw);
                    TryBeforeHandlers(ctx);
                    TryEndpointHandler(ctx);
                    TryErrorHandler(ctx);
                    TryAfterHandlers(ctx);

                    if (config.enableCorsForAllOrigins) ctx.Header("Access-Control-Allow-Origin", "*");
                    SendResponse(contextRaw.Response, ctx);
                }
            }).Start();
        }

        public void AddErrorHandler(int statusCode, Handler handler) {
            errorMapper.Add(statusCode, handler);
        }

        public void AddHandler(HandlerType type, string path, Handler handler) {
            matcher.Add(new HandlerEntry(type, path, config.ignoreTrailingSlashes, handler));
        }

        private void TryWithExceptionMapper(Context ctx, Action Func) {
            exceptionMapper.CatchException(ctx, Func);
        }

        private void TryBeforeHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var beforeEntries = matcher.FindEntries(HandlerType.BEFORE, ctx.Path());
            beforeEntries.ForEach(entry => {
                entry.handler(ContextUtil.Update(ctx, entry));
            });
        });

        private void TryErrorHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            errorMapper.Handle(ctx.Status(), ctx);
        });

        private void TryEndpointHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var type = Enum.Parse<HandlerType>(ctx.Method());
            var entries = matcher.FindEntries(type, ctx.Path());
            var entry = entries.FindLast(entry => true);

            if (entry == null) throw new NotFoundResponse();

            entry.handler(ContextUtil.Update(ctx, entry));
        });

        private void TryAfterHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var afterEntries = matcher.FindEntries(HandlerType.AFTER, ctx.Path());
            afterEntries.ForEach(entry => {
                entry.handler(ContextUtil.Update(ctx, entry));
            });
        });

        private void SendResponse(HttpListenerResponse response, Context ctx) {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ctx.ResultString());
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            Console.WriteLine($"[{ctx.Method()}] {ctx.Status()} {ctx.Path()}");
        }
    }
}
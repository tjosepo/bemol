using System;
using System.Linq;
using System.Net;
using System.Threading;
using Bemol.Core;
using Bemol.Http.Util;
using Bemol.Http.Exceptions;

namespace Bemol.Http {
    public class BemolServer {
        public int Port { set; get; } = 7000;
        public string Host { set; get; } = "localhost";
        public bool Started { set; get; } = false;

        private BemolConfig Config;
        private BemolRenderer Renderer;
        private StaticFilesHandler StaticFilesHandler;
        private PathMatcher Matcher = new PathMatcher();
        private ErrorMapper ErrorMapper = new ErrorMapper();
        private ExceptionMapper ExceptionMapper = new ExceptionMapper();

        public BemolServer(BemolConfig config) {
            Renderer = new BemolRenderer(config);
            StaticFilesHandler = new StaticFilesHandler(config);
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
                    HandleRequest(ctx);
                    SendResponse(ctx);
                }
            }).Start();
        }

        public void HandleRequest(Context ctx) {
            ctx.Renderer = Renderer;
            ctx.ContentType(Config.DefaultContentType);

            TryBeforeHandlers(ctx);
            TryEndpointHandler(ctx);
            if (ctx.Status() == 404) {
                TryStaticFiles(ctx);
            }
            TryErrorHandler(ctx);
            TryAfterHandlers(ctx);

            if (Config.EnableCorsForAllOrigins) ctx.Header("Access-Control-Allow-Origin", "*");
        }

        public void AddHandler(HandlerType type, string path, Handler handler) {
            Matcher.Add(new HandlerEntry(type, path, Config.IgnoreTrailingSlashes, handler));
        }

        public void AddErrorHandler(int statusCode, Handler handler) {
            ErrorMapper.Add(statusCode, handler);
        }

        private void TryWithExceptionMapper(Context ctx, Action func) {
            ExceptionMapper.CatchException(ctx, func);
        }

        private void TryBeforeHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var beforeEntries = Matcher.FindEntries(HandlerType.BEFORE, ctx.Path());
            beforeEntries.ForEach(entry => {
                entry.Handle(ContextUtil.Update(ctx, entry));
            });
        });

        private void TryEndpointHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var type = Enum.Parse<HandlerType>(ctx.Method());
            var entries = Matcher.FindEntries(type, ctx.Path());
            if (entries.Count == 0) throw new NotFoundException();

            var entry = entries.Last();
            entry.Handle(ContextUtil.Update(ctx, entry));
        });

        private void TryStaticFiles(Context ctx) => TryWithExceptionMapper(ctx, () => {
            if (ctx.Method() == "GET") {
                StaticFilesHandler.Handle(ctx);
            }
        });

        private void TryErrorHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            ErrorMapper.Handle(ctx.Status(), ctx);
        });

        private void TryAfterHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var afterEntries = Matcher.FindEntries(HandlerType.AFTER, ctx.Path());
            afterEntries.ForEach(entry => {
                entry.Handle(ContextUtil.Update(ctx, entry));
            });
        });

        private void SendResponse(Context ctx) {
            var resultStream = ctx.ResultStream();
            resultStream.Write(ctx.ResultBytes());
            resultStream.Close();
        }
    }
}
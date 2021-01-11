using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Bemol.Core;
using Bemol.Http.Util;
using Bemol.Http.Exceptions;

namespace Bemol.Http {
    internal class BemolServer {
        internal int Port = 7000;
        internal string Host = "localhost";
        internal bool Started = false;

        private readonly BemolConfig Config;
        private readonly BemolRenderer Renderer;
        private readonly StaticFilesHandler StaticFilesHandler;
        private readonly PathMatcher Matcher = new PathMatcher();
        private readonly ErrorMapper ErrorMapper = new ErrorMapper();
        private readonly ExceptionMapper ExceptionMapper = new ExceptionMapper();

        internal BemolServer(BemolConfig config) {
            Config = config;
            Renderer = new BemolRenderer(config);
            StaticFilesHandler = new StaticFilesHandler(config);
        }

        internal void Start() {
            new Thread(() => {
                var listener = new HttpListener();
                listener.Prefixes.Add($"http://{Host}:{Port}{Config.ContextPath}");
                listener.Start();

                Console.Clear();
                Console.WriteLine($"Listening on http://{Host}:{Port}{Config.ContextPath}");

                while (Started) {
                    var rawCtx = listener.GetContext();
                    new Task(() => {
                        var ctx = new Context(rawCtx, Config);
                        HandleRequest(ctx);
                        SendResponse(ctx);
                    }).Start();
                }
            }).Start();
        }

        internal void HandleRequest(Context ctx) {
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

        internal void AddHandler(HandlerType type, string path, Handler handler) {
            Matcher.Add(new HandlerEntry(type, path, Config.IgnoreTrailingSlashes, handler));
        }

        internal void AddErrorHandler(int statusCode, Handler handler) {
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
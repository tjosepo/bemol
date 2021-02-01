using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bemol.Http;
using Bemol.Http.Util;
using Bemol.Http.Exceptions;

namespace Bemol.Core {
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

        internal virtual void Start() {
            new Thread(() => {
                var listener = new HttpListener();
                listener.Prefixes.Add($"http://{Host}:{Port}{Config.ContextPath}");
                listener.Start();

                // Console.Clear();
                Console.WriteLine($"Listening on http://{Host}:{Port}{Config.ContextPath}");

                while (Started) {
                    var rawCtx = listener.GetContext();
                    new Task(() => {
                        var request = new Bemol.Core.Server.HttpListener.HttpListenerRequest(rawCtx);
                        var response = new Bemol.Core.Server.HttpListener.HttpListenerResponse(rawCtx);
                        var ctx = new Context(request, response);
                        HandleRequest(ctx);
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

            SendResponse(ctx);
        }

        internal void AddHandler(string method, string path, Handler handler, RouterConfig config) {
            Matcher.Add(new HandlerEntry(method, path, handler, config));
        }

        internal void AddErrorHandler(int statusCode, Handler handler) {
            ErrorMapper.Add(statusCode, handler);
        }

        private void TryWithExceptionMapper(Context ctx, Action func) {
            ExceptionMapper.CatchException(ctx, func);
        }

        private void TryBeforeHandlers(Context ctx) => TryWithExceptionMapper(ctx, () => {
            TryHandlers("BEFORE", ctx);
        });

        private void TryEndpointHandler(Context ctx) => TryWithExceptionMapper(ctx, () => {
            var entries = TryHandlers(ctx.Method(), ctx);
            if (entries == null || entries?.Count == 0) throw new NotFoundException();
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
            TryHandlers("AFTER", ctx);
        });

        private List<HandlerEntry>? TryHandlers(string method, Context ctx) {
            var entries = Matcher.FindEntries(method, ctx.Path());
            entries?.ForEach(entry => {
                entry.Handle(ContextUtil.Update(ctx, entry));
            });
            return entries;
        }

        private void SendResponse(Context ctx) {
            var resultStream = ctx.ResultStream();
            resultStream.Write(ctx.ResultBytes());
            resultStream.Dispose();
            resultStream.Close();
        }
    }
}
using System;
using System.Net;
using System.Threading;
using Bemol.Core;

namespace Bemol.Http {
    class BemolServer {
        private BemolConfig config;
        public int port { set; get; } = 7000;
        public string host { set; get; } = "localhost";
        public bool started { set; get; } = false;
        private PathMatcher matcher = new PathMatcher();

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

                    if (config.enableCorsForAllOrigins) ctx.Header("Access-Control-Allow-Origin", "*");

                    TryBeforeHandlers(ctx);
                    TryEndpointHandler(ctx);
                    TryAfterHandlers(ctx);

                    SendResponse(contextRaw.Response, ctx);

                    Console.WriteLine($"[{ctx.Method()}] {ctx.Status()} {ctx.Path()}");
                }
            }).Start();
        }

        public void AddHandler(HandlerType type, string path, Handler handler) {
            matcher.Add(new HandlerEntry(type, path, config.ignoreTrailingSlashes, handler));
        }

        private void TryBeforeHandlers(Context ctx) {
            var beforeEntries = matcher.FindEntries(HandlerType.BEFORE, ctx.Path());
            beforeEntries.ForEach(beforeEntry => beforeEntry.handler(ctx));
        }

        private void TryEndpointHandler(Context ctx) {
            var type = Enum.Parse<HandlerType>(ctx.Method());
            var entries = matcher.FindEntries(type, ctx.Path());
            var entry = entries.FindLast(entry => true);

            if (entry == null) {
                entry = new HandlerEntry(HandlerType.INVALID, "", config.ignoreTrailingSlashes, (ctx) => ctx.Result("404 Not found").Status(404));
            }

            entry.handler(ctx);
        }

        private void TryAfterHandlers(Context ctx) {
            var afterEntries = matcher.FindEntries(HandlerType.AFTER, ctx.Path());
            afterEntries.ForEach(beforeEntry => beforeEntry.handler(ctx));
        }

        private void SendResponse(HttpListenerResponse response, Context ctx) {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ctx.ResultString());
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Bemol.Core;

namespace Bemol.Http {
    class BemolServer {
        private BemolConfig config;
        public int port { set; get; } = 7000;
        public string host { set; get; } = "localhost";
        public bool started { set; get; } = false;
        private PathMatcher matcher = new PathMatcher();
        private HttpListener listener;
        private IDictionary handlers;

        public BemolServer(BemolConfig config) {
            this.config = config;
        }

        public void Start() {
            listener = new HttpListener();
            handlers = new Dictionary<(HandlerType, string), Handler>();

            Thread th = new Thread(() => {
                listener.Start();
                Console.Clear();
                Console.WriteLine($"Listening on http://{host}:{port}{config.contextPath}");
                listener.Prefixes.Add($"http://{host}:{port}{config.contextPath}");

                while (true) {
                    var context = listener.GetContext();
                    var type = Enum.Parse<HandlerType>(context.Request.HttpMethod);
                    string path = context.Request.RawUrl;
                    var handlers = matcher.FindEntries(type, path);
                    var entry = handlers.FindLast(entry => true);

                    if (entry == null) {
                        entry = new HandlerEntry(HandlerType.INVALID, "", config.ignoreTrailingSlashes, (ctx) => ctx.Result("404 Not found").Status(404));
                    }

                    Context ctx = new Context(context);
                    entry.handler(ctx);

                    HttpListenerResponse response = context.Response;

                    if (config.enableCorsForAllOrigins) response.AppendHeader("Access-Control-Allow-Origin", "*");
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ctx.ResultString());
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            });
            th.Start();
        }

        public void addHandler(HandlerType type, string path, Handler handler) {
            matcher.Add(new HandlerEntry(type, path, config.ignoreTrailingSlashes, handler));
        }
    }
}
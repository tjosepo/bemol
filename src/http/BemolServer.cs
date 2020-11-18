using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Bemol {
    class BemolServer {
        private int port;
        private string host = "localhost";
        private HttpListener listener;
        private IDictionary handlers;

        public void Start() {
            port = 7000;

            listener = new HttpListener();
            handlers = new Dictionary<(string, string), Handler>();

            Thread th = new Thread(() => {
                listener.Start();
                Console.WriteLine($"Listening on http://{host}:{port}/");

                while (true) {
                    if (listener.Prefixes.Count == 0) {
                        Thread.Yield();
                        continue;
                    }
                    var ctx = listener.GetContext();
                    var method = ctx.Request.HttpMethod;
                    var path = ctx.Request.RawUrl;
                    var handler = (Handler)handlers[(method, path)];
                    if (handler == null) {
                        handler = (ctx) => ctx.Result("404 Not found");
                    }
                    handler(new Context(ctx));
                }
            });
            th.Start();
        }

        public void addHandler(string method, string path, Handler handler) {
            listener.Prefixes.Add($"http://{host}:{port}{path}");
            handlers.Add((method, path), handler);
        }
    }
}
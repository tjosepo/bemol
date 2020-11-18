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

        public void Start(int port) {
            this.port = port;

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
                    var context = listener.GetContext();
                    var method = context.Request.HttpMethod;
                    var path = context.Request.RawUrl;
                    var handler = handlers[(method, path)] as Handler;
                    if (handler == null) {
                        handler = (ctx) => ctx.Status(404).Result("404 Not found");
                    }
                    Context ctx = new Context(context);
                    handler(ctx);

                    var response = context.Response;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ctx.ResultString());
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            });
            th.Start();
        }

        public void addHandler(string method, string path, Handler handler) {
            if (!listener.Prefixes.Contains($"http://{host}:{port}{path}"))
                listener.Prefixes.Add($"http://{host}:{port}{path}");
            handlers.Add((method, path), handler);
        }
    }
}
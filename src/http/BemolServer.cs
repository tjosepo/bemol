using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Bemol {
    class BemolServer {
        private int port;
        private string host;
        private HttpListener listener;
        private IDictionary handlers;

        public void Start(string host, int port) {
            this.host = host;
            this.port = port;

            listener = new HttpListener();
            handlers = new Dictionary<(HandlerType, string), Handler>();

            Thread th = new Thread(() => {
                listener.Start();
                Console.WriteLine($"Listening on http://{host}:{port}/");

                while (true) {
                    if (listener.Prefixes.Count == 0) {
                        Thread.Yield();
                        continue;
                    }

                    var context = listener.GetContext();
                    var type = Enum.Parse<HandlerType>(context.Request.HttpMethod);
                    var path = context.Request.RawUrl;
                    var handler = handlers[(type, path)] as Handler;

                    if (handler == null) {
                        handler = (ctx) => ctx.Result("404 Not found").Status(404);
                    }

                    Context ctx = new Context(context);
                    handler(ctx);

                    HttpListenerResponse response = context.Response;
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
            if (!listener.Prefixes.Contains($"http://{host}:{port}{path}"))
                listener.Prefixes.Add($"http://{host}:{port}{path}");
            handlers.Add((type, path), handler);
        }
    }
}
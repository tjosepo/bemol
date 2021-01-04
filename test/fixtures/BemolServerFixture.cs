using System;
using System.Net;
using System.Net.Http;

using Bemol.Http;

namespace Bemol.Test.Fixtures {
    public class BemolServerFixture : IDisposable {
        private readonly int Port;
        private readonly HttpListener Listener = new HttpListener();
        private readonly HttpClient Client = new HttpClient();

        public BemolServerFixture() {
            Port = 7357;
            string address = $"http://localhost:{Port}/";
            Listener.Prefixes.Add(address);
            Listener.Start();
            Client.BaseAddress = new Uri(address);
            Client.Timeout = new TimeSpan(300 * 100000);
        }

        public Context GetContext(ClientHandler handler) {
            handler.Invoke(Client);
            var rawCtx = Listener.GetContext();
            var ctx = new Context(rawCtx);
            return ctx;
        }

        public void SendResponse(Context ctx) {
            var resultStream = ctx.ResultStream();
            resultStream.Write(ctx.ResultBytes());
            resultStream.Close();
        }

        public delegate void ClientHandler(HttpClient client);

        public void Dispose() {
            Listener.Abort();
            Client.Dispose();
        }
    }
}
using System;
using System.Net;
using System.Net.Http;
using Bemol.Http;

namespace Bemol.Test.Fixtures {
    public class BemolServerFixture : IDisposable {
        private readonly int Port;
        private HttpListener Listener = new HttpListener();
        private HttpClient Client = new HttpClient();

        public BemolServerFixture() {
            Port = 7357;
            var address = $"http://localhost:{Port}/";
            Listener.Prefixes.Add(address);
            Listener.Start();
            Client.BaseAddress = new Uri(address);
        }

        public Context GetContext(ClientHandler handler) {
            handler(Client);
            var rawCtx = Listener.GetContext();
            Context ctx = new Context(rawCtx);
            return ctx;
        }

        public delegate void ClientHandler(HttpClient client);

        public void Dispose() {
            Listener.Abort();
            Client.Dispose();
        }
    }
}
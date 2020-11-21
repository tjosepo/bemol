using System;
using Bemol.Core;
using Bemol.Http;

namespace Bemol {
    public class App {
        BemolConfig config = new BemolConfig();

        BemolServer server;

        public App() {
            server = new BemolServer(config);
        }

        public App(Action<BemolConfig> config) : this() {
            config(this.config);
        }

        public App Start() {
            if (server.started) {
                var message = @"Server already started. If you are trying to call Start() on an instance 
                of App that was stopped using Stop(), please create a new instance instead.";
                throw new InvalidOperationException(message);
            }
            server.started = true;
            server.Start();
            return this;
        }

        public App Start(int port) {
            server.port = port;
            return Start();
        }

        public App Start(string host, int port) {
            server.host = host;
            return Start(port);
        }

        // ********************************************************************************************
        // HTTP
        // ********************************************************************************************

        public App Error(int statusCode, Handler handler) {
            return this;
        }

        public App Get(string path, Handler handler) {
            server.addHandler(HandlerType.GET, path, handler);
            return this;
        }

        public App Post(string path, Handler handler) {
            server.addHandler(HandlerType.POST, path, handler);
            return this;
        }
    }
}
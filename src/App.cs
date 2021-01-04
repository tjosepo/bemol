using System;

using Bemol.Core;
using Bemol.Http;

namespace Bemol {
    public class App {
        private readonly BemolServer Server;
        private readonly BemolConfig Config = new BemolConfig();

        public App() => Server = new BemolServer(Config);

        public App(Action<BemolConfig> config) : this() => config(Config);

        /// <summary> Starts the application instance on the default port (7000). </summary>
        public App Start() {
            if (Server.Started) {
                var message = @"Server already started. If you are trying to call Start() on an instance 
                of App that was stopped using Stop(), please create a new instance instead.";
                throw new InvalidOperationException(message);
            }
            Server.Started = true;
            Server.Start();
            return this;
        }

        /// <summary> Starts the application instance on the specified port. </summary>
        public App Start(int port) {
            Server.Port = port;
            return Start();
        }

        /// <summary> 
        /// Starts the application instance on the specified port
        /// with the given host IP to bind to. 
        /// </summary>
        public App Start(string host, int port) {
            Server.Host = host;
            return Start(port);
        }

        /// <summary> Stops the application instance. </summary>
        public App Stop() {
            Server.Started = false;
            return this;
        }

        // ********************************************************************************************
        // HTTP
        // ********************************************************************************************

        public App Error(int statusCode, Handler handler) {
            Server.AddErrorHandler(statusCode, handler);
            return this;
        }

        /// <summary> Adds a GET request handler for the specified path to the instance. </summary>
        public App Get(string path, Handler handler) {
            Server.AddHandler(HandlerType.GET, path, handler);
            return this;
        }

        /// <summary> Adds a POST request handler for the specified path to the instance. </summary>
        public App Post(string path, Handler handler) {
            Server.AddHandler(HandlerType.POST, path, handler);
            return this;
        }

        // ********************************************************************************************
        // BEFORE / AFTER
        // ********************************************************************************************

        /// <summary> Adds a BEFORE request handler for the specified path to the instance. </summary>
        public App Before(string path, Handler handler) {
            Server.AddHandler(HandlerType.BEFORE, path, handler);
            return this;
        }

        /// <summary> Adds a BEFORE request handler for all routes in the instance. </summary>
        public App Before(Handler handler) => Before("*", handler);

        /// <summary> Adds an AFTER request handler for the specified path to the instance. </summary>
        public App After(string path, Handler handler) {
            Server.AddHandler(HandlerType.AFTER, path, handler);
            return this;
        }

        /// <summary> Adds an AFTER request handler for all routes in the instance. </summary>
        public App After(Handler handler) => After("*", handler);
    }
}
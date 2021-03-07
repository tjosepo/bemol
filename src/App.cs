using System;

using Bemol.Core;
using Bemol.Http;

namespace Bemol {
    public class App {
        private readonly BemolServer Server;
        private readonly BemolConfig Config = new BemolConfig();

        public App() => Server = new BemolServer(Config);

        public App(Action<BemolConfig> config) : this() => config.Invoke(Config);

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

        /// <summary> Adds an error handler for the specified status code. </summary>
        public App Error(int statusCode, Handler handler) {
            Server.AddErrorHandler(statusCode, handler);
            return this;
        }

        /// <summary> Adds a custom request handler for the specified method and path to the instance. </summary>
        public App Route(string method, string path, Handler handler) {
            Server.AddHandler(method.ToUpper(), path, handler, Config);
            return this;
        }

        /// <summary> Adds a GET request handler for the specified path to the instance. </summary>
        public App Get(string path, Handler handler) => Route("GET", path, handler);

        /// <summary> Adds a HEAD request handler for the specified path to the instance. </summary>
        public App Head(string path, Handler handler) => Route("HEAD", path, handler);

        /// <summary> Adds a POST request handler for the specified path to the instance. </summary>
        public App Post(string path, Handler handler) => Route("POST", path, handler);

        /// <summary> Adds a PUT request handler for the specified path to the instance. </summary>
        public App Put(string path, Handler handler) => Route("PUT", path, handler);

        /// <summary> Adds a PATCH request handler for the specified path to the instance. </summary>
        public App Patch(string path, Handler handler) => Route("PATCH", path, handler);

        /// <summary> Adds a DELETE request handler for the specified path to the instance. </summary>
        public App Delete(string path, Handler handler) => Route("DELETE", path, handler);

        // ********************************************************************************************
        // MIDDLEWARES
        // ********************************************************************************************

        /// <summary> Adds a middleware for all routes in the instance. </summary>
        public App Use(Handler handler) => Use("*", handler);

        /// <summary> Adds a middleware for the specified path to the instance. </summary>
        public App Use(string path, Handler handler) => Route("USE", path, handler);

        /// <summary> Adds a router to the application instance. </summary>
        public App Use(Router router) {
            foreach (var (method, path, handler) in router.Routes) {
                Server.AddHandler(method.ToUpper(), path, handler, router.Config);
            }
            return this;
        }

        /// <summary> Adds a router with the specified path prefix to the application instance. </summary>
        public App Use(Router router, string prefix) {
            foreach (var (method, path, handler) in router.Routes) {
                Server.AddHandler(method.ToUpper(), $"{prefix}/{path}", handler, router.Config);
            }
            return this;
        }
    }
}
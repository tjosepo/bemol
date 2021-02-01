using System;
using System.Collections.Generic;

using Bemol.Http;
using Bemol.Core;

namespace Bemol {
    public class Router {
        internal readonly List<(string method, string path, Handler handler)> Routes = new List<(string, string, Handler)>();
        internal readonly RouterConfig Config = new RouterConfig();

        public Router(Action<RouterConfig> config) => config.Invoke(Config);

        // ********************************************************************************************
        // HTTP
        // ********************************************************************************************

        /// <summary> Adds a custom request handler for the specified method and path to the instance. </summary>
        public Router Route(string method, string path, Handler handler) {
            Routes.Add((method, path, handler));
            return this;
        }

        /// <summary> Adds a GET request handler for the specified path to the instance. </summary>
        public Router Get(string path, Handler handler) => Route("GET", path, handler);

        /// <summary> Adds a HEAD request handler for the specified path to the instance. </summary>
        public Router Head(string path, Handler handler) => Route("HEAD", path, handler);

        /// <summary> Adds a POST request handler for the specified path to the instance. </summary>
        public Router Post(string path, Handler handler) => Route("POST", path, handler);

        /// <summary> Adds a PUT request handler for the specified path to the instance. </summary>
        public Router Put(string path, Handler handler) => Route("PUT", path, handler);

        /// <summary> Adds a PATCH request handler for the specified path to the instance. </summary>
        public Router Patch(string path, Handler handler) => Route("PATCH", path, handler);

        /// <summary> Adds a DELETE request handler for the specified path to the instance. </summary>
        public Router Delete(string path, Handler handler) => Route("DELETE", path, handler);

        // ********************************************************************************************
        // BEFORE / AFTER
        // ********************************************************************************************

        /// <summary> Adds a BEFORE request handler for the specified path to the instance. </summary>
        public Router Before(string path, Handler handler) => Route("BEFORE", path, handler);

        /// <summary> Adds a BEFORE request handler for all routes in the instance. </summary>
        public Router Before(Handler handler) => Before("*", handler);

        /// <summary> Adds an AFTER request handler for the specified path to the instance. </summary>
        public Router After(string path, Handler handler) => Route("AFTER", path, handler);

        /// <summary> Adds an AFTER request handler for all routes in the instance. </summary>
        public Router After(Handler handler) => After("*", handler);
    }
}
using System;

using Bemol.Core;
using Bemol.Http;

namespace Bemol {
  public class App : Router {
    private readonly BemolServer Server;
    new private readonly BemolConfig Config = new BemolConfig();

    public App() => Server = new BemolServer(this, Config);

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

    // ********************************************************************************************
    // MIDDLEWARES
    // ********************************************************************************************

    /// <summary> Adds a middleware for all routes in the instance. </summary>
    public App Use(Handler handler) => Use("*", handler);

    /// <summary> Adds a middleware for the specified path to the instance. </summary>
    public App Use(string path, Handler handler) => (App)Add("USE", path, handler);
  }
}
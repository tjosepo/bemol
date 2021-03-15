using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Bemol.Http;
using Bemol.Http.Util;
using Bemol.Routing;

namespace Bemol.Core {
  internal class BemolServer {
    internal int Port = 7000;
    internal string Host = "localhost";
    internal bool Started = false;

    private readonly BemolConfig Config;
    private readonly BemolRenderer Renderer;
    private readonly StaticFilesHandler StaticFilesHandler;
    private readonly Router Router;
    private readonly ErrorMapper ErrorMapper = new ErrorMapper();
    private readonly ExceptionMapper ExceptionMapper = new ExceptionMapper();

    internal BemolServer(Router router, BemolConfig config) {
      Router = router;
      Config = config;
      Renderer = new BemolRenderer(config);
      StaticFilesHandler = new StaticFilesHandler(config);
    }

    internal virtual void Start() {
      new Thread(() => {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://{Host}:{Port}{Config.ContextPath}");
        listener.Start();

        Console.Clear();
        Console.WriteLine($"Listening on http://{Host}:{Port}{Config.ContextPath}");

        while (Started) {
          var rawCtx = listener.GetContext();
          Task.Run(() => {
            var request = new Bemol.Core.Server.HttpListener.HttpListenerRequest(rawCtx);
            var response = new Bemol.Core.Server.HttpListener.HttpListenerResponse(rawCtx);
            var ctx = new Context(request, response);
            HandleRequest(ctx);
          });
        }
      }).Start();
    }

    internal void HandleRequest(Context ctx) {
      ctx.ContentType(Config.DefaultContentType);

      TryWithExceptionMapper(ctx, () => {
        TryEndpointHandler(ctx);
      });

      TryErrorHandler(ctx);
      TryFinally(ctx);

      SendResponse(ctx);
    }

    internal void AddErrorHandler(int statusCode, Handler handler) {
      ErrorMapper.Add(statusCode, handler);
    }

    private void TryWithExceptionMapper(Context ctx, Action func) {
      ExceptionMapper.CatchException(ctx, func);
    }

    private void TryEndpointHandler(Context ctx) {
      var entries = TryHandlers(ctx.Method(), ctx);
      if (entries.Count == 0) TryStaticFiles(ctx);
    }

    private void TryStaticFiles(Context ctx) {
      if (ctx.Method() == "GET") {
        StaticFilesHandler.Handle(ctx);
      } else {
        ctx.Status(404);
      }
    }

    private void TryErrorHandler(Context ctx) {
      ErrorMapper.Handle(ctx.Status(), ctx);
    }

    private List<Route> TryHandlers(string method, Context ctx) {
      var routes = Router.Find(method, ctx.Path());
      var middlewares = new List<Middleware>();
      routes.ForEach(route => middlewares.AddRange(route.Middlewares));
      middlewares.ForEach(middleware => middleware.Handler(ctx));
      routes.ForEach(route => {
        route.Handler.Invoke(ContextUtil.Update(ctx, route));
      });
      return routes;
    }

    private void TryFinally(Context ctx) {
      while (ctx.Waitlist.Count > 0) {
        var next = ctx.Waitlist.Pop();
        TryWithExceptionMapper(ctx, () => {
          next.Invoke();
        });
      }
    }

    private void SendResponse(Context ctx) {
      var resultStream = ctx.ResultStream();
      resultStream.Write(ctx.ResultBytes());
      resultStream.Dispose();
      resultStream.Close();
    }
  }
}
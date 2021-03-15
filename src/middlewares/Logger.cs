using System;

using Bemol.Http;

namespace Bemol.Middlewares {
  public class Logger : Middleware {

    public Logger() : base(Log) { }

    private static void Log(Context ctx) {
      ctx.Finally(() => {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = GetForegroundColor(ctx);
        var message = $"[{ctx.Status()}] {ctx.Method()} {ctx.Path()} {ctx.Ip()}";
        Console.WriteLine(message);
        Console.ForegroundColor = foregroundColor;
      });
    }

    private static ConsoleColor GetForegroundColor(Context ctx) {
      switch (ctx.Status()) {
        case int status when (status >= 200 && status < 300):
          return ConsoleColor.Green;
        case int status when (status >= 300 && status < 400):
          return ConsoleColor.DarkYellow;
        case int status when (status >= 400 && status < 600):
          return ConsoleColor.Red;
        default:
          return ConsoleColor.White;
      }
    }

    // public class LoggerConfig {
    //   public Func<Context, String> Format = (ctx) => $"[{ctx.Status()}] {ctx.Method()} {ctx.Path()} {ctx.Ip()}";
    // }
  }
}
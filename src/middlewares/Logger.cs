using System;

using Bemol.Http;

namespace Bemol.Middlewares {
    public class Logger {
        public static Handler New() => New(new Config());

        public static Handler New(Action<Config> configure) {
            Config config = new Config();
            configure(config);
            return New(config);
        }

        public static Handler New(Config config) => new Handler((ctx) => {
            ctx.Finally(() => {
                var foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = GetForegroundColor(ctx);
                var message = config.Format(ctx);
                Console.WriteLine(message);
                Console.ForegroundColor = foregroundColor;
            });
        });

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

        public class Config {
            public Func<Context, String> Format = (ctx) => $"[{ctx.Status()}] {ctx.Method()} {ctx.Path()} {ctx.Ip()}";
        }
    }
}
using System;

using Bemol.Http;

namespace Bemol.Middlewares {
    public class Cors {
        public static Handler New() => New(new Config());

        public static Handler New(Action<Config> configure) {
            Config config = new Config();
            configure(config);
            return New(config);
        }

        public static Handler New(Config config) => new Handler(ctx => {
            ctx.Header("Access-Control-Allow-Origin", config.AllowOrigins);
        });

        public class Config {
            /// <summary> Defines a list of origins that may access the resource. </summary>
            public string AllowOrigins = "*";
        }
    }
}
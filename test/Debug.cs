using Bemol;

public class Debug {
    public static void Main() {
        App app = new App(config => {
            config.DefaultContentType = "application/json";
        }).Start();

        app.Get("/", ctx => {
            ctx.Render("/index.liquid", new { });
        });

        app.Post("/", ctx => {
            var body = ctx.Body();
            ctx.Result(body);
        });
    }
}
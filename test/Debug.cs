using Bemol;

public class Debug {
    public static void Main() {
        App app = new App().Start();

        app.Get("/", ctx => {
            ctx.Render("/index.liquid", new { });
        });

        app.Post("/", ctx => {
            var body = ctx.Body();
            ctx.Result(body);
        });

        var renderer = new Bemol.Core.BemolRenderer(new Bemol.Core.BemolConfig());
        renderer.Render("/index.liquid");
    }
}
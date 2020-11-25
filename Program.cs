using Bemol;

public class Program {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => ctx.Html("Hello World!"));
        app.Get("/user/:name", ctx => ctx.Html($"Hello {ctx.PathParam("bob")}."));
        app.Error(404, ctx => ctx.Html("404 Error: Page not found"));
    }
}
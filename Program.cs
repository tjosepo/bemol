using Bemol;

public class Program {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => ctx.Html("Hello world!"));
        app.Get("/user/:name", ctx => ctx.Html($"Hello {ctx.PathParam("name")}."));
    }
}
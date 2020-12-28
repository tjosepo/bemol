using Bemol;
using System.Net;

public class Program {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => {
            int num = 0;
            num = (2 / num);
        });
        app.Get("/user/:name", ctx => ctx.Html($"Hello {ctx.PathParam("bob")}."));
        app.Error(404, ctx => ctx.Html("404 Error: Page not found"));
        app.Error(500, ctx => ctx.Html("Who there! You did an oopsy!"));
    }
}
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
    }
}
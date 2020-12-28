using Bemol;
using Bemol.Http.Exceptions;

public class Program {
    public static void Main() {
        App app = new App(config => {
            config.DefaultContentType = "application/json";
        }).Start();
        app.Get("/", ctx => throw new NotFoundException());
        app.Get("/user/:name", ctx => ctx.Html($"Hello {ctx.PathParam("name")}."));
        app.Error(404, ctx => throw new InternalServerErrorException());
        app.Error(500, ctx => ctx.Result("An oopsy occured!"));
    }
}
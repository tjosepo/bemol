using Bemol;

public class Program {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => ctx.Html("Hello World!"));
        app.Get("/stop", _ => app.Stop());
    }
}
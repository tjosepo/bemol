using Bemol;

public class Debug {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => ctx.Render("/index.liquid"));
    }
}
using Bemol;

public class Program {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => ctx.Result("Hello world"));
    }
}
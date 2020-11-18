using Bemol;

public class Progam {
    public static void Main() {
        App app = new Bemol.App().Start();
        app.Get("/", (ctx) => ctx.Result("Hello World!"));
    }
}
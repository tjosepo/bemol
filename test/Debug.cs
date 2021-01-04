using Bemol;

public class Debug {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => {
            ctx.Html($"Content type: {ctx.ContentType()}");
        });

        app.Get("/add", ctx => {
            ctx.Cookie("Added", "True");
            ctx.Html($"Cookie added!");
        });

        app.Get("/check", ctx => {
            ctx.Html($"Cookie value: {ctx.Cookie("Added")}.");
        });

        app.Get("/remove", ctx => {
            ctx.RemoveCookie("Added");
            ctx.Html($"Cookie Removed.");
        });
    }
}
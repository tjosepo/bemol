using Bemol;
using System.IO;

public class Debug {
    public static void Main() {
        App app = new App().Start();
        app.Get("/", ctx => {
            ctx.Render("index.liquid");
        });

        app.Post("/", ctx => {
            var file = ctx.UploadedFile("file");
            using (var fileStream = File.Create($"./{file.Filename}")) {
                file.Content.Seek(0, SeekOrigin.Begin);
                file.Content.CopyTo(fileStream);
            }

            ctx.Result($"uploaded: {file.Filename} ({file.Size / 1024}kB)");
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
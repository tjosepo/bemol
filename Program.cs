using System;
using Bemol;

public class Progam {
    public static void Main() {
        App app = new Bemol.App().Start(7000);
        app.Get("/", (ctx) => ctx.Result("Hello World!"));
        app.Get("/json/", (ctx) => ctx.Json(new User("tim", 2)));
        app.Get("/cookie/", (ctx) => {
            ctx.Cookie("Foooo");
            ctx.Result("Cookie has been set");
        });
        app.Post("/", (ctx) => {
            Console.WriteLine(ctx.Body());
        });

        app.Post("/get-user/", (ctx) => {
            Console.WriteLine(ctx.BodyAsClass<User>());
        });
    }
}

public class User {
    public string name { set; get; }
    public int id { set; get; }

    public User(string name, int id) {
        this.name = name;
        this.id = id;
    }
}
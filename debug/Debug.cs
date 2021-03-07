using Bemol;
using Bemol.Middlewares;

using System;

using Wesbite;

App app = new App().Start();

app.Get("/", ctx => {
    System.Console.WriteLine("endpoint handler");
    ctx.Render("index.liquid");
});
app.Post("/", ctx => ctx.Render("index.liquid"));
// app.Use(new Auth().Router);

app.Use(Logger.New());
app.Use((ctx) => {
    var start = DateTime.Now;
    ctx.Finally(() => {
        var ms = DateTime.Now - start;
        Console.WriteLine("Response time:" + ms);
    });
});





using Bemol;

using Wesbite;

App app = new App().Start();
app.Get("/", ctx => ctx.Render("index.liquid"));
app.Use(new Auth().Router);

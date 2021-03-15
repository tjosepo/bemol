using Bemol;
using Bemol.Middlewares;

App app = new App().Start();

app.Get("/", ctx => {
  ctx.Render("index.liquid");
});
app.Post("/", ctx => ctx.Render("index.liquid"));
app.Add("auth", new Auth());

app.Add(new Logger());
app.Add(new Cors());


app.Post("/", ctx => {
  var file = ctx.UploadedFile("file");
});


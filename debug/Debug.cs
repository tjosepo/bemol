using Bemol;
using Bemol.Middlewares;

App app = new App().Start();

Router router = new Router();

router.Get("/hello", (ctx) => {
  ctx.Result("Hello world!");
});

app.Add(router);

using Bemol;
using Bemol.Http;

public class Auth : Router {

  public Auth() : base(config => {
    config.TemplateFolder = "auth/templates";
  }) {
    Get("/login", ctx => ctx.Render("/login.liquid"));
    Post("/login", Login);
  }

  private void Login(Context ctx) {
  }
}

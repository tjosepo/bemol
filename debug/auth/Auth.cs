using Bemol;
using Bemol.Http;

namespace Wesbite {
    public class Auth {
        public Router Router;

        public Auth() {
            Router = new Router(config => {
                config.TemplateFolder = "/auth/templates";
            });

            Router.Get("/login", ctx => ctx.Render("/login.liquid"));
            Router.Post("/login", Login);
        }

        private void Login(Context ctx) {
        }
    }
}
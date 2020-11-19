namespace Bemol {
    public class App {
        BemolServer server = new BemolServer();

        public App Start() {
            return Start("localhost", 7000);
        }

        public App Start(int port) {
            return Start("localhost", port);
        }

        public App Start(string localhost, int port) {
            server.Start(localhost, port);
            return this;
        }

        public void Get(string path, Handler handler) {
            server.addHandler(HandlerType.GET, path, handler);
        }

        public void Post(string path, Handler handler) {
            server.addHandler(HandlerType.POST, path, handler);
        }
    }
}
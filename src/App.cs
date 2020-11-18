namespace Bemol {
    public class App {

        BemolServer server;

        public App Start() {
            return Start(7000);
        }
        public App Start(int port) {
            server = new BemolServer();
            server.Start(port);

            return this;
        }
        public void Get(string path, Handler handler) {
            server.addHandler("GET", path, handler);
        }

        public void Post(string path, Handler handler) {
            server.addHandler("POST", path, handler);
        }
    }
}
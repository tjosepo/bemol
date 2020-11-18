namespace Bemol {
    public class App {

        BemolServer server;

        public App Start() {
            server = new BemolServer();
            server.Start();

            return this;
        }
        public void Get(string path, Handler handler) {
            server.addHandler("GET", path, handler);
        }
    }
}
using System.Net;

namespace Bemol {
    public class Context {
        private HttpListenerContext ctx;

        public Context(HttpListenerContext ctx) {
            this.ctx = ctx;
        }

        public void Result(string resultString) {
            HttpListenerRequest request = ctx.Request;
            // Obtain a response object.
            HttpListenerResponse response = ctx.Response;
            // Construct a response.
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(resultString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
    }
}
using System.IO;
using System.Net;
using System.Text.Json;

namespace Bemol.Http {
    public class Context {
        private HttpListenerRequest request;
        private HttpListenerResponse response;
        private string resultString = "";

        public Context(HttpListenerContext ctx) {
            request = ctx.Request;
            response = ctx.Response;
        }

        // ********************************************************************************************
        // REQUEST
        // ********************************************************************************************

        public string Body() {
            System.IO.Stream stream = request.InputStream;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public T BodyAsClass<T>() {
            string json = Body();
            return JsonSerializer.Deserialize<T>(json);
        }

        public string PathParam(string key) {
            return key;
        }

        public Cookie Cookie(string name) {
            return request.Cookies[name];
        }

        // ********************************************************************************************
        // RESPONSE
        // ********************************************************************************************

        public Context Result(string resultString) {
            this.resultString = resultString;
            return this;
        }

        public string ResultString() {
            return resultString;
        }

        public Context ContentType(string contentType) {
            response.ContentType = contentType;
            return this;
        }

        public Context Status(int statusCode) {
            response.StatusCode = statusCode;
            return this;
        }

        public int Status() {
            return response.StatusCode;
        }

        public Context Cookie(string name, string value) {
            var cookie = new Cookie(name, value, "/");
            response.SetCookie(cookie);
            return this;
        }

        public Context Cookie(Cookie cookie) {
            response.SetCookie(cookie);
            return this;
        }

        public Context Html(string html) {
            return Result(html).ContentType("text/html");
        }

        public Context Json(object obj) {
            return Result(JsonSerializer.Serialize(obj)).ContentType("application/json");
        }
    }
}
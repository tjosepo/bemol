using System.Net;
using System.Collections.Generic;
using System.Text.Json;
using Bemol.Http.Exceptions;

namespace Bemol.Http {

    /// <summary> Provides access to functions for handling the request and response.</summary>
    public class Context {
        public Dictionary<string, string> PathParamDict { get; set; }
        internal HttpListenerRequest Request;
        internal HttpListenerResponse Response;
        private string ResultString = "";

        public Context(HttpListenerContext ctx) {
            Request = ctx.Request;
            Response = ctx.Response;
        }

        // ********************************************************************************************
        // REQUEST
        // ********************************************************************************************

        /// <summary> Gets the request body as a <paramref name="string"/>. </summary>
        public string Body() {
            System.IO.Stream stream = Request.InputStream;
            var reader = new System.IO.StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary> Maps a JSON body to a class using JsonSerializer. </summary>
        /// <returns> The mapped object </returns>
        public T BodyAsClass<T>() {
            string json = Body();
            return JsonSerializer.Deserialize<T>(json);
        }

        public string PathParam(string key) {
            if (!PathParamDict.ContainsKey(key)) throw new InternalServerErrorException($"'{key}' is not a valid path-param for '{Path()}'.");
            return PathParamDict[key];
        }

        /// <summary> Gets a request cookie by name, or null. <summary>
        public Cookie Cookie(string name) => Request.Cookies[name];

        /// <summary> Gets the request method. <summary>
        public string Method() => Request.HttpMethod;

        /// <summary> Gets the request absolute path. </summary>
        public string Path() => Request.Url.AbsolutePath;

        // ********************************************************************************************
        // RESPONSE
        // ********************************************************************************************

        /// <summary> Set a <paramref name="string"/> result that will be sent to the client. </summary>
        public Context Result(string resultString) {
            ResultString = resultString;
            return this;
        }

        /// <summary> Get the string result that will be sent to the client </summary>
        public string Result() => ResultString;

        /// <summary> Sets response content type to specified <paramref name="string"> value. </summary>
        public Context ContentType(string contentType) {
            Response.ContentType = contentType;
            return this;
        }

        /// <summary> Sets response header by name and value. </summary>
        public Context Header(string name, string value) {
            Response.AddHeader(name, value);
            return this;
        }

        /// <summary> Sets the response status. </summary>
        public Context Status(int statusCode) {
            Response.StatusCode = statusCode;
            return this;
        }

        /// <summary> Gets the response status. </summary>
        public int Status() => Response.StatusCode;

        /// <summary> Sets a cookie with name and value. </summary>
        public Context Cookie(string name, string value) {
            var cookie = new Cookie(name, value, "/");
            Response.SetCookie(cookie);
            return this;
        }

        /// <summary> Sets a cookie. </summary>
        public Context Cookie(Cookie cookie) {
            Response.SetCookie(cookie);
            return this;
        }

        /// <summary> 
        /// Sets context result to specified html string 
        /// and sets content-type to <c>text/html</c>.
        /// </summary>  
        public Context Html(string html) => Result(html).ContentType("text/html");

        /// <summary>
        /// Serializes object to a JSON-string and sets it as the context result.
        /// Sets content type to <c>application/json</c>.
        /// </summary>     
        public Context Json(object obj) => Result(JsonSerializer.Serialize(obj)).ContentType("application/json");
    }
}
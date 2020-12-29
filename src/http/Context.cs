using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.IO;

using Bemol.Core;
using Bemol.Http.Util;
using Bemol.Http.Exceptions;

namespace Bemol.Http {

    /// <summary> Provides access to functions for handling the request and response.</summary>
    public class Context {

        internal HttpListenerRequest Request;
        internal HttpListenerResponse Response;

        private Dictionary<string, string> Form;
        public Dictionary<string, string> PathParamDict { get; set; }

        private byte[] BodyArray;
        private byte[] ResultArray;

        public Context(HttpListenerContext ctx) {
            Request = ctx.Request;
            Response = ctx.Response;
        }

        // ********************************************************************************************
        // REQUEST
        // ********************************************************************************************

        /// <summary> Gets the request body as a <paramref name="string"/>. </summary>
        public string Body() {
            var bytes = (BodyArray != null) ? BodyArray : BodyAsBytes();
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary> Gets the request body as a an array of bytes. </summary>
        public byte[] BodyAsBytes() {
            if (BodyArray != null) return BodyArray;
            using (MemoryStream ms = new MemoryStream()) {
                Request.InputStream.CopyTo(ms);
                BodyArray = ms.ToArray();
                return BodyArray;
            }
        }

        /// <summary> 
        /// Maps a JSON body to a class using JsonSerializer. 
        /// Throws <c>BadRequestException</c> if the object cannot be mapped. 
        /// </summary>
        /// <returns> The mapped object </returns>
        public T BodyAsClass<T>() {
            string json = Body();
            try {
                return JsonSerializer.Deserialize<T>(json);
            } catch (JsonException) {
                throw new BadRequestException();
            }
        }
        /// <summary> Gets a form param if it exists, else null. </summary>
        public string FormParam(string key) {
            if (Form == null) Form = ContextUtil.SplitKeyValueStringAndGroupByKey(Body());
            return Form[key];
        }

        /// <summary> Gets a form param if it exists, else a default value (null if not specified explicitly).</summary>
        public string FormParam(string key, string defaultValue = null) {
            var value = FormParam(key);
            return (value != null) ? value : defaultValue;
        }

        /// <summary> 
        /// Gets a path param by name. 
        /// Throws <c>InternalServerErrorException</c> if the path param cannot be found. 
        /// </summary>
        public string PathParam(string key) {
            if (!PathParamDict.ContainsKey(key)) throw new InternalServerErrorException($"'{key}' is not a valid path-param for '{Path()}'.");
            return PathParamDict[key];
        }

        /// <summary> Gets the request content length. </summary>
        public long ContentLength() => Request.ContentLength64;

        /// <summary> Gets the request content type, or null. </summary>
        public string ContentType() => Request.ContentType;

        /// <summary> Gets a request cookie by name, or null. </summary>
        public Cookie Cookie(string name) => Request.Cookies[name];

        /// <summary> Gets a request header by name, or null. </summary>
        public string Header(string header) => HeaderMap()[header];

        /// <summary> Gets a map with all the header keys and values on the request. </summary>
        public NameValueCollection HeaderMap() => Request.Headers;

        /// <summary> Gets the request method. </summary>
        public string Method() => Request.HttpMethod;

        /// <summary> Gets the request absolute path. </summary>
        public string Path() => Request.Url.AbsolutePath;

        // ********************************************************************************************
        // RESPONSE
        // ********************************************************************************************

        /// <summary> Set a <paramref name="string"/> result that will be sent to the client. </summary>
        public Context Result(string resultString) => Result(Encoding.UTF8.GetBytes(resultString));

        /// <summary> Get the string result that will be sent to the client. </summary>
        public string ResultString() => Encoding.UTF8.GetString(ResultArray);

        /// <summary> Set an array of bytes result that will be sent to the client. </summary>
        public Context Result(byte[] byteArray) {
            ResultArray = byteArray;
            return this;
        }

        /// <summary> Get the array of byte result that will be sent to the client. </summary>
        public byte[] ResultBytes() => ResultArray;

        /// <summary> Get the array of byte result that will be sent to the client. </summary>
        public Stream ResultStream() => Response.OutputStream;

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

        /// <summary>
        /// Renders a Liquid file located in the <c>resources</c> folder with specified values and sets it as the context result.
        /// Also sets content-type to text/html.
        /// </summary>
        public Context Render(string filePath, object model) => Html(BemolRenderer.Render(filePath, model));
    }
}
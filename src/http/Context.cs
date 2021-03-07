using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

using Bemol.Core;
using Bemol.Http.Util;
using Bemol.Http.Exceptions;

namespace Bemol.Http {

    /// <summary> Provides access to functions for handling the request and response.</summary>
    public class Context {
        private readonly IRequest Request;
        private readonly IResponse Response;

        private Form? Form;
        private NameValueCollection? Query;
        internal Dictionary<string, string>? PathParamDict;
        internal BemolRenderer? Renderer;

        private byte[] BodyArray = null!;  // Because byte?[] to byte[] conversion is a pain
        private byte[] ResultArray = null!;  // Because byte?[] to byte[] conversion is a pain

        internal Context(IRequest request, IResponse response) {
            Request = request;
            Response = response;
        }

        // ********************************************************************************************
        // NEXT
        // ********************************************************************************************

        internal Stack<Action> Waitlist = new Stack<Action>();

        public void Finally(Action handler) => Waitlist.Push(handler);

        // ********************************************************************************************
        // REQUEST
        // ********************************************************************************************

        /// <summary> Gets the request body as a <paramref name="string"/>. </summary>
        public string Body() {
            byte[] bytes = BodyArray ?? BodyAsBytes();
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary> 
        /// Maps a JSON body to a class using JsonSerializer.
        /// </summary>
        /// <returns> The mapped object </returns>
        /// <exception cref="BadRequestException">Thrown when the body cannot be converted to <paramref name="T" /></exception>
        public T? Body<T>() {
            try {
                return JsonSerializer.Deserialize<T>(Body());
            } catch (JsonException) {
                throw new BadRequestException();
            }
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

        /// <summary> Gets first <c>UploadedFile</c> for the specified name, or null. </summary>
        public UploadedFile? UploadedFile(string name) {
            Form ??= new Form(this);
            return Form.Files?[name];
        }

        /// <summary> Gets a form param if it exists, else null. </summary>
        public string? FormParam(string key) => FormParam()[key];

        /// <summary> 
        /// Gets a form param of type <paramref name="T"/> if it exists, else throw.
        /// </summary>
        /// <exception cref="BadRequestException">Thrown when the form param cannot be converted to <paramref name="T" /></exception>
        public T FormParam<T>(string key) {
            try {
                return (T)Convert.ChangeType(FormParam(key)!, typeof(T));
            } catch (Exception) {
                throw new BadRequestException();
            }
        }

        /// <summary> Gets a form param if it exists, else a default value (null if not specified explicitly).</summary>
        public string? FormParam(string key, string? defaultValue = null) {
            var value = FormParam(key);
            return value ?? defaultValue;
        }

        /// <summary> Gets a collection with all the form param keys and values. </summary>
        public NameValueCollection FormParam() {
            Form ??= new Form(this);
            return Form.Parameters;
        }

        /// <summary> 
        /// Gets a path param by name if it exists, else throw.
        /// </summary>
        /// <exception cref="InternalServerErrorException">Thrown when the path param cannot be found.</exception>
        public string PathParam(string key) {
            if (PathParamDict is null) throw new InternalServerErrorException("Cannot use 'PathParam' method inside of the current context.");
            if (!PathParamDict!.ContainsKey(key)) throw new InternalServerErrorException($"'{key}' is not a valid path-param for '{Path()}'.");
            return PathParamDict[key];
        }

        /// <summary> 
        /// Gets a path param of type <paramref name="T"/> by name if it exists, else throw
        /// </summary>
        /// <exception cref="BadRequestException">Thrown when the path param cannot be converted to <paramref name="T" /></exception>
        /// <exception cref="InternalServerErrorException">Thrown when the path param cannot be found.</exception>
        public T PathParam<T>(string key) {
            try {
                return (T)Convert.ChangeType(PathParam(key), typeof(T));
            } catch (FormatException) {
                throw new BadRequestException();
            }
        }

        /// <summary> Gets the request content length. </summary>
        public long? ContentLength() => Request.ContentLength;

        /// <summary> Gets the request content type, or null. </summary>
        public string? ContentType() => Request.ContentType;

        /// <summary> Gets a request cookie by name, or null. </summary>
        public string? Cookie(string name) => Request.Cookies[name];

        /// <summary> Gets a request header by name, or null. </summary>
        public string? Header(string header) => Header()[header];

        /// <summary> Gets a collection with all the header keys and values on the request. </summary>
        public NameValueCollection Header() => Request.Headers;

        /// <summary> Gets the request ip. </summary>
        public string Ip() => Request.Ip;

        /// <summary> Returns true if request is multipart/form-data </summary>
        public bool IsMultipartFormData() => Request.ContentType?.Contains("multipart/form-data") ?? false;

        /// <summary> Gets the request method. </summary>
        public string Method() => Request.Method;

        /// <summary> Gets the request absolute path. </summary>
        public string Path() => Request.Path;

        /// <summary> Gets a comma separated string with all for params of the specified key, or null. </summary>
        public string? QueryParam(string key) => QueryMap()[key];

        /// <summary>
        /// Gets a comma separated string with all for params of the specified key of type <paramref name="T">, or throw.
        /// </summary>
        /// <exception cref="BadRequestException">Thrown when the query param cannot be converted to <paramref name="T" /></exception>
        public T QueryParam<T>(string key) {
            try {
                return (T)Convert.ChangeType(QueryParam(key)!, typeof(T));
            } catch (Exception) {
                throw new BadRequestException();
            }
        }

        /// <summary> Gets a collection with all the query param keys and values. </summary>
        public NameValueCollection QueryMap() {
            Query ??= ContextUtil.SplitKeyValueString(QueryString());
            return Query;
        }

        /// <summary> Gets the request query string. </summary>
        public string QueryString() => Request.Query;

        /// <summary> Gets the request user agent. </summary>
        public string UserAgent() => Request.UserAgent;

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

        /// <summary> Redirects to the specified path. </summary>
        public Context Redirect(string path) => Redirect(path, 302);

        /// <summary> Redirects to the specified path with specifix HTTP 3XX status code. </summary>
        public Context Redirect(string path, int statusCode) => Header("Location", path).Status(statusCode);

        /// <summary> Sets a cookie with name and value. </summary>
        public Context Cookie(string name, string value) => Cookie(new Cookie(name, value));

        /// <summary> Sets a cookie. </summary>
        public Context Cookie(Cookie cookie) {
            Response.SetCookie(cookie);
            return this;
        }

        /// <summary> Removes cookie specified by name. </summary>
        public Context RemoveCookie(string name) => Cookie(new Cookie(name, null) { Expired = true });

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
        public Context Render(string filePath, object? model = null) => Html(Renderer!.Render(filePath, model));
    }
}
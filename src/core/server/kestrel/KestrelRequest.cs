using System.IO;
using System.Collections.Specialized;

using Microsoft.AspNetCore.Http;

using Bemol.Http;

namespace Bemol.Core.Server.Kestrel {
    public sealed class KestrelRequest : IRequest {
        private readonly HttpRequest Request;

        public KestrelRequest(HttpContext context) {
            Request = context.Request;
        }

        public Stream InputStream { get => Request.Body; }
        public long? ContentLength { get => Request.ContentLength; }
        public string ContentType { get => Request.ContentType; }

        public NameValueCollection Cookies {
            get {
                var cookies = new NameValueCollection();
                foreach (var cookie in Request.Cookies) cookies.Add(cookie.Key, cookie.Value);
                return cookies;
            }
        }

        public NameValueCollection Headers {
            get {
                var headers = new NameValueCollection();
                foreach (var header in Request.Headers) headers.Add(header.Key, header.Value);
                return headers;
            }
        }

        public string Ip { get => Request.Host.Value; }
        public string Method { get => Request.Method; }
        public string Path { get => Request.Path; }
        public string Query { get => Request.QueryString.ToUriComponent(); }
        public string UserAgent { get => Request.Headers["User-Agent"]; }
    }
}
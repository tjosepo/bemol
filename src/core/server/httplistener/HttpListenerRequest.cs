using System.IO;
using System.Net;
using System.Collections.Specialized;

using Bemol.Http;

namespace Bemol.Core.Server.HttpListener {
    public sealed class HttpListernerRequest : IRequest {
        private readonly System.Net.HttpListenerRequest Request;

        public HttpListernerRequest(HttpListenerContext context) {
            Request = context.Request;
        }

        public Stream InputStream { get => Request.InputStream; }
        public long? ContentLength { get => Request.ContentLength64; }
        public string? ContentType { get => Request.ContentType; }

        public NameValueCollection Cookies {
            get {
                var cookies = new NameValueCollection();
                foreach (Cookie cookie in Request.Cookies) cookies.Add(cookie.Name, cookie.Value);
                return cookies;
            }
        }

        public NameValueCollection Headers { get => Request.Headers; }
        public string Ip { get => Request.UserHostAddress; }
        public string Method { get => Request.HttpMethod; }
        public string Path { get => Request.Url!.AbsolutePath; }
        public string Query { get => Request.Url!.Query; }
        public string UserAgent { get => Request.UserAgent; }
    }
}
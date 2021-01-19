using System.IO;
using System.Net;

using Bemol.Http;

namespace Bemol.Core.Server.HttpListener {
    public sealed class HttpListenerResponse : IResponse {
        private readonly System.Net.HttpListenerResponse Response;

        public HttpListenerResponse(HttpListenerContext context) {
            Response = context.Response;
        }

        public Stream OutputStream { get => Response.OutputStream; }
        public string? ContentType { set => Response.ContentType = value; get => Response.ContentType; }
        public int StatusCode { set => Response.StatusCode = value; get => Response.StatusCode; }

        public void AddHeader(string name, string value) => Response.AddHeader(name, value);
        public void SetCookie(Cookie cookie) => Response.SetCookie(cookie);
    }
}
using System.IO;
using System.Net;

using Microsoft.AspNetCore.Http;

using Bemol.Http;

namespace Bemol.Core.Server.Kestrel {
    public sealed class KestrelResponse : IResponse {
        private readonly HttpResponse Response;

        public KestrelResponse(HttpContext context) {
            Response = context.Response;
        }

        public Stream OutputStream { get => Response.Body; }
        public string? ContentType { set => Response.ContentType = value ?? ""; get => Response.ContentType; }
        public int StatusCode { set => Response.StatusCode = value; get => Response.StatusCode; }

        public void AddHeader(string name, string value) => Response.Headers.Add(name, value);
        public void SetCookie(Cookie cookie) => Response.Cookies.Append(cookie.Name, cookie.Value);
    }
}
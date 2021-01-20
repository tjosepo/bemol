using System.IO;
using System.Net;

namespace Bemol.Http {
    public interface IResponse {
        Stream OutputStream { get; }
        string? ContentType { set; get; }
        int StatusCode { set; get; }

        void AddHeader(string name, string value);
        void SetCookie(Cookie cookie);
    }
}
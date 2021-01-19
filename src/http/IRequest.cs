using System.IO;
using System.Collections.Specialized;

namespace Bemol.Http {
    interface IRequest {
        Stream InputStream { get; }
        long? ContentLength { get; }
        string? ContentType { get; }
        NameValueCollection Cookies { get; }
        NameValueCollection Headers { get; }
        string Ip { get; }
        string Method { get; }
        string Path { get; }
        string Query { get; }
        string UserAgent { get; }
    }
}
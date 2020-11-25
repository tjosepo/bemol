using System;
using System.Net;

namespace Bemol.Http {
    public class HttpStatusException : Exception {
        public HttpStatusCode statusCode { get; }
        public string message { get; }

        public HttpStatusException(HttpStatusCode statusCode, string message) {
            this.statusCode = statusCode;
            this.message = message;
        }
    }
}
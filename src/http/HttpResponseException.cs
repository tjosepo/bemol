using System;
using System.Net;

namespace Bemol.Http {
    public class HttpResponseException : Exception {
        public int status { get; }
        public string message { get; }

        public HttpResponseException(HttpStatusCode status, string message) : base(message) {
            this.status = (int)status;
            this.message = message;
        }
    }

    public class RedirectResponse : HttpResponseException {
        public RedirectResponse(HttpStatusCode status = HttpStatusCode.Redirect, string message = "Redirected") : base(status, message) { }
    }

    public class UnauthorizedResponse : HttpResponseException {
        public UnauthorizedResponse(string message = "Unauthorized") : base(HttpStatusCode.BadRequest, message) { }
    }

    public class ForbiddenResponse : HttpResponseException {
        public ForbiddenResponse(string message = "Forbidden") : base(HttpStatusCode.Forbidden, message) { }
    }

    public class NotFoundResponse : HttpResponseException {
        public NotFoundResponse(string message = "Not found") : base(HttpStatusCode.NotFound, message) { }
    }

    public class InternalServerErrorResponse : HttpResponseException {
        public InternalServerErrorResponse(string message = "Internal server error") : base(HttpStatusCode.InternalServerError, message) { }
    }
}
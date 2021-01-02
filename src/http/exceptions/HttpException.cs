namespace Bemol.Http.Exceptions {
    public class HttpException : System.Exception {
        public readonly int Status;
        public HttpException(int status, string message) : base(message) {
            Status = status;
        }
    }

    public class RedirectResponse : HttpException {
        public RedirectResponse(int status = 300, string message = "Redirected") : base(status, message) { }
    }

    public class BadRequestException : HttpException {
        public BadRequestException(string message = "Bad Request") : base(400, message) { }
    }

    public class UnauthorizedException : HttpException {
        public UnauthorizedException(string message = "Unauthorized") : base(401, message) { }
    }

    public class ForbiddenException : HttpException {
        public ForbiddenException(string message = "Forbidden") : base(403, message) { }
    }

    public class NotFoundException : HttpException {
        public NotFoundException(string message = "Not Found") : base(404, message) { }
    }

    public class MethodNotAllowedException : HttpException {
        public MethodNotAllowedException(string message = "Method Not Allowed") : base(405, message) { }
    }

    public class InternalServerErrorException : HttpException {
        public InternalServerErrorException(string message = "Internal Server Error") : base(500, message) { }
    }
}
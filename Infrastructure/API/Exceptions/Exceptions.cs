namespace RealEstate.Infrastructure.API.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found.
    /// Maps to HTTP 404 Not Found.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when a request is invalid.
    /// Maps to HTTP 400 Bad Request.
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown for unexpected internal errors.
    /// Maps to HTTP 500 Internal Server Error.
    /// </summary>
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}

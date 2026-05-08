namespace Core.Exceptions;

using System.Net;

public class TmsException : Exception
{
    public TmsException(string message)
        : base(message)
    {
    }

    public TmsException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public TmsException(string message, Exception inner, HttpStatusCode? statusCode)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }

    public TmsException(string message, HttpStatusCode? statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode? StatusCode { get; }
}
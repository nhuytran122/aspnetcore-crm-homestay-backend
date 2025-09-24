using System.Globalization;
using System.Net;

namespace CRM_Homestay.Core.Exceptions;

public class GlobalException : Exception
{
    public const string ErrorCode = "error_code";
    public const string StatusCode = "status_code";
    public const string ErrorList = "errors";
    public GlobalException() : base()
    {
    }
    public GlobalException(string message) : base(message)
    {
    }

    public GlobalException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public GlobalException(string message, HttpStatusCode code) : base(message)
    {
        Data.Add(ErrorCode, code);
    }

    public GlobalException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture,
        message, args))
    {
    }

    public GlobalException(string code, string message, HttpStatusCode statusCode, List<string>? errors = null)
        : base(message)
    {
        Data[ErrorCode] = code;
        Data[StatusCode] = (int)statusCode;
        Data[ErrorList] = errors ?? new List<string>();
    }
}
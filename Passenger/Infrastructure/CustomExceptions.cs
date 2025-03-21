using System.Net;
using System.Runtime.Serialization;
using System.Security;

[Serializable]
public class ApiRequestException : Exception
{
    public string Endpoint { get; }
    public HttpStatusCode StatusCode { get; }

    public ApiRequestException(string endpoint, HttpStatusCode statusCode, string message)
        : base($"API request to {endpoint} failed with status {(int)statusCode} ({statusCode}): {message}")
    {
        Endpoint = endpoint;
        StatusCode = statusCode;
    }

    // Serialization constructor
    protected ApiRequestException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
        Endpoint = info.GetString(nameof(Endpoint))!;
        StatusCode = (HttpStatusCode)info.GetInt32(nameof(StatusCode));
    }

    [SecurityCritical]
    public override void GetObjectData(
        SerializationInfo info,
        StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Endpoint), Endpoint);
        info.AddValue(nameof(StatusCode), (int)StatusCode); // Store enum as its integer value
    }
}
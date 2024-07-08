using System.Net;
using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Response
{
    [JsonPropertyName("status")]
    public long Status { get; set; }

    [JsonPropertyName("statusText")]
    public string StatusText { get; set; }

    [JsonPropertyName("httpVersion")]
    public string HttpVersion { get; set; }

    [JsonPropertyName("headers")]
    public Cooky[] Headers { get; set; }

    [JsonPropertyName("cookies")]
    public Cooky[] Cookies { get; set; }

    [JsonPropertyName("content")]
    public Content Content { get; set; }

    [JsonPropertyName("redirectURL")]
    public string RedirectUrl { get; set; }

    [JsonPropertyName("headersSize")]
    public long HeadersSize { get; set; }

    [JsonPropertyName("bodySize")]
    public long BodySize { get; set; }
}
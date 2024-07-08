using System.Net;
using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Request
{
    [JsonPropertyName("bodySize")]
    public long BodySize { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("httpVersion")]
    public string HttpVersion { get; set; }

    [JsonPropertyName("headers")]
    public Cooky[] Headers { get; set; }

    [JsonPropertyName("cookies")]
    public Cooky[] Cookies { get; set; }

    [JsonPropertyName("queryString")]
    public Cooky[] QueryString { get; set; }

    [JsonPropertyName("headersSize")]
    public long HeadersSize { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("postData")]
    public PostData PostData { get; set; }
}
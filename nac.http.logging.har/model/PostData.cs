using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class PostData
{
    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; }

    [JsonPropertyName("params")]
    public object[] Params { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class PageTimings
{
    [JsonPropertyName("onContentLoad")]
    public long OnContentLoad { get; set; }

    [JsonPropertyName("onLoad")]
    public long OnLoad { get; set; }
}
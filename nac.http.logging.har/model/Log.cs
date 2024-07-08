using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Log
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("creator")]
    public Browser Creator { get; set; }

    [JsonPropertyName("browser")]
    public Browser Browser { get; set; }

    [JsonPropertyName("pages")]
    public Page[] Pages { get; set; }

    [JsonPropertyName("entries")]
    public Entry[] Entries { get; set; }
}
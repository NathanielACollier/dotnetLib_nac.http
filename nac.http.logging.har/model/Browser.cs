using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Browser
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }
}
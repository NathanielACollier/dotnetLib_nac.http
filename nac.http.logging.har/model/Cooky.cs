using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Cooky
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}
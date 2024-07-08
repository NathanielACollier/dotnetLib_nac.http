using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class HARFileRoot
{
    [JsonPropertyName("log")]
    public Log Log { get; set; }
}
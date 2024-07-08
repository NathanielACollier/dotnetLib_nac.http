using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Timings
{
    [JsonPropertyName("blocked")]
    public long Blocked { get; set; }

    [JsonPropertyName("dns")]
    public long Dns { get; set; }

    [JsonPropertyName("connect")]
    public long Connect { get; set; }

    [JsonPropertyName("ssl")]
    public long Ssl { get; set; }

    [JsonPropertyName("send")]
    public long Send { get; set; }

    [JsonPropertyName("wait")]
    public long Wait { get; set; }

    [JsonPropertyName("receive")]
    public long Receive { get; set; }
}
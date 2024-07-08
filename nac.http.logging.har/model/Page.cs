using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Page
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("pageTimings")]
    public PageTimings PageTimings { get; set; }

    [JsonPropertyName("startedDateTime")]
    public DateTimeOffset StartedDateTime { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
}
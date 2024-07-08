using System;
using System.Text.Json.Serialization;

namespace nac.http.logging.har.model;

public class Entry
{
    [JsonPropertyName("startedDateTime")]
    public DateTimeOffset StartedDateTime { get; set; }

    [JsonPropertyName("request")]
    public Request Request { get; set; }

    [JsonPropertyName("response")]
    public Response Response { get; set; }

    [JsonPropertyName("cache")]
    public Cache Cache { get; set; }

    [JsonPropertyName("timings")]
    public Timings Timings { get; set; }

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("_securityState")]
    public string SecurityState { get; set; }

    [JsonPropertyName("serverIPAddress")]
    public string ServerIpAddress { get; set; }

    [JsonPropertyName("connection")]
    public string Connection { get; set; }

    [JsonPropertyName("pageref")]
    public string Pageref { get; set; }
}
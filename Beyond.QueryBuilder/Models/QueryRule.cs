// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace Beyond.QueryBuilder.Models;

[Serializable]
public class QueryRule
{
    [JsonPropertyName("connector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Connector { get; set; }

    [JsonPropertyName("operator")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Operator { get; set; }

    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Property { get; set; }

    [JsonPropertyName("rules")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<QueryRule>? Rules { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Value { get; set; }

    //[JsonPropertyName("not")]
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //public bool Not { get; set; }
}
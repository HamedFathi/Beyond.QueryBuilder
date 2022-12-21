// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace Beyond.QueryBuilder.Models;

[Serializable]
public class Query
{
    [JsonPropertyName("connector")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Connector { get; set; }

    [JsonPropertyName("rules")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<QueryRule>? Rules { get; set; }

    //[JsonPropertyName("orderBy")]
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //public List<OrderBy>? OrderBy { get; set; }

    //[JsonPropertyName("not")]
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //public bool Not { get; set; }

    public static explicit operator QueryRule(Query query)
    {
        return new QueryRule { Connector = query.Connector, Rules = query.Rules };
    }
}
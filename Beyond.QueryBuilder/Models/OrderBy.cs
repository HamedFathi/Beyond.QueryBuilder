// ReSharper disable UnusedMember.Global

using System.Text.Json.Serialization;

namespace Beyond.QueryBuilder.Models;

[Serializable]
public class OrderBy
{
    [JsonPropertyName("ascending")]
    public bool Ascending { get; set; }

    [JsonPropertyName("property")] public string Property { get; set; } = null!;
}
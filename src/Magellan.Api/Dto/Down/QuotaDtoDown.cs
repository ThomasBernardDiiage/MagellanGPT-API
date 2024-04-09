using System.Text.Json.Serialization;

namespace Magellan.Api.Dto.Down;

public record QuotaDtoDown
{
    [JsonPropertyName("currentQuota")]
    public int CurrentQuota { get; set; }
    
    [JsonPropertyName("maxQuota")]
    public int MaxQuota { get; set; }
};
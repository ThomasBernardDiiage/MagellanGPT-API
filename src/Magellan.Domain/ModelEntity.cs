using Newtonsoft.Json;

namespace Magellan.Domain;

public class ModelEntity
{
    [JsonProperty("id")] 
    public string Id { get; set; } = "";

    [JsonProperty("name")] 
    public string Name { get; set; } = "";

    [JsonProperty("index")]
    public int Index { get; set; }
}
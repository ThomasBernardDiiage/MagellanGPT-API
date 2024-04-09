using Newtonsoft.Json;

namespace Magellan.Domain;

public class FileEntity
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("mediaUrl")]
    public string MediaUrl { get; set; }
    
    [JsonProperty("date")]
    public DateTime Date { get; set; }
}
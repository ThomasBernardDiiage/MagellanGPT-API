using Newtonsoft.Json;

namespace Magellan.Domain;

public class MessageEntity
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    
    [JsonProperty("sender")]
    public MessageSender Sender { get; set; }

    [JsonProperty("text")] 
    public string Text { get; set; } = "";
    
    [JsonProperty("model")]
    public string? Model { get; set; }
    
    [JsonProperty("files_names")]
    public List<string> FilesNames { get; set; } = new();
    
    [JsonProperty("date")]
    public DateTime Date { get; set; }
    
}
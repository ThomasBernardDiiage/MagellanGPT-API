using Newtonsoft.Json;

namespace Magellan.Domain;

public class ConversationEntity
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    
    [JsonProperty("last_modification_date")]
    public DateTime LastModificationDate { get; set; }

    [JsonProperty("prompt")] 
    public string Prompt { get; set; } = "";
    

    [JsonProperty("messages")] 
    public List<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
    
    
    [JsonProperty("files")]
    public List<FileEntity> Files { get; set; } = new List<FileEntity>();
}
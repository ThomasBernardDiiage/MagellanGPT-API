using Newtonsoft.Json;

namespace Magellan.Domain;

public class UserEntity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("conversations")]
    public List<ConversationEntity> Conversations { get; set; } = new List<ConversationEntity>();
}
using Magellan.Domain;

namespace Magellan.DataAccess.Interfaces;

public interface IConversationRepository
{
    public Task<IEnumerable<ConversationEntity>> GetConversationsAsync(string userId);
    
    public Task<ConversationEntity> GetConversation(string userId, Guid conversationId);
    public Task DeleteConversation(string userId, Guid conversationId);
    public Task<IEnumerable<MessageEntity>> GetConversationMessages(string userId, Guid conversationId);
    
    public Task<ConversationEntity> CreateConversation(string userId, string title, string prompt = "");

    public Task<ConversationEntity> SaveMessageInConversation(string userId, Guid conversationId, MessageEntity message);
    
    
    public Task<bool> ConversationExists(string userId, Guid conversationId);
}
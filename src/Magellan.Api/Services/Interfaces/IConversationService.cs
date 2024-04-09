using Magellan.Api.Dto.Down;
using Magellan.Domain;

namespace Magellan.Api.Services.Interfaces;

public interface IConversationService
{
    public Task<MessageEntity> SendMessageInConversation(string userId, Guid conversationId, string message, string model);
    public Task<MessageEntity> SendMessageInConversation(string userId, Guid conversationId, string message, List<IFormFile> files, string model);
}
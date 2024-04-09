using Magellan.DataAccess.Interfaces;
using Magellan.Domain;
using Magellan.Domain.Exceptions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.KernelMemory;

namespace Magellan.DataAccess;

public class ConversationRepository : IConversationRepository
{
    private readonly Container _container;
    
    public ConversationRepository(Container container)
    {
        _container = container;
    }
    
    private async Task<UserEntity> GetUser(string userId)
    {
        var query = _container.GetItemLinqQueryable<UserEntity>()
            .Where(it => it.Id == userId)
            .Take(1)
            .ToQueryDefinition();

        var user = (await _container.GetItemQueryIterator<UserEntity>(query).ReadNextAsync()).FirstOrDefault();

        if (user is null)
            throw new UserNotExistsException();
        
        return user;
    }
    
    public async Task<IEnumerable<ConversationEntity>> GetConversationsAsync(string userId)
    {
        try
        {
            var user = await GetUser(userId);
            return user.Conversations;
        }
        catch (UserNotExistsException _)
        {
            return new List<ConversationEntity>();
        }
    }

    public async Task<ConversationEntity> GetConversation(string userId, Guid conversationId)
    {
        var user = await GetUser(userId);

        var conversation = user.Conversations.Find(it => it.Id == conversationId);

        if (conversation is null)
            throw new ConversationNotExistsException();
        
        return conversation;
    }
    
    public async Task DeleteConversation(string userId, Guid conversationId)
    {
        var user = await GetUser(userId); // Throw exception if user not exists
        await GetConversation(userId, conversationId); // Throw exception if conversation not exists
        user.Conversations.RemoveAll(it => it.Id == conversationId);
        await _container.ReplaceItemAsync(user, user.Id);
    }

    public async Task<IEnumerable<MessageEntity>> GetConversationMessages(string userId, Guid conversationId)
    {
        return (await GetConversation(userId, conversationId)).Messages;
    }

    public async Task<ConversationEntity> CreateConversation(string userId, string title, string prompt = "")
    {
        UserEntity? user;
        var conversation = new ConversationEntity
        {
            Id = Guid.NewGuid(),
            Title = title,
            Prompt = prompt,
            Messages = new List<MessageEntity>(),
            LastModificationDate = DateTime.Now
        };

        if (!string.IsNullOrEmpty(prompt))
        {
            var message = new MessageEntity
            {
                Id = Guid.NewGuid(),
                Text = prompt,
                Date = DateTime.Now,
                Sender = MessageSender.Prompt
            };
            conversation.Messages.Add(message);
        }
        
        
        try
        { 
            user = await GetUser(userId);
        }
        catch (UserNotExistsException e)
        {
            var result = await _container.CreateItemAsync(new UserEntity { Id = userId});
            user = result.Resource;
        }
        
        user.Conversations.Add(conversation);

        await _container.ReplaceItemAsync(user, user.Id);

        return conversation;
    }

    public async Task<ConversationEntity> SaveMessageInConversation(string userId, Guid conversationId, MessageEntity message)
    {
        var user = await GetUser(userId);
        await GetConversation(userId, conversationId);
        
        user.Conversations
            .First(it => it.Id == conversationId).Messages.Add(message);
        
        user.Conversations
            .First(it => it.Id == conversationId).LastModificationDate = DateTime.Now;

        await _container.ReplaceItemAsync(user, user.Id);
        return user.Conversations.First(it => it.Id == conversationId);
    }

    public async Task<bool> ConversationExists(string userId, Guid conversationId)
    {
        var user = await GetUser(userId);
        return user.Conversations.Any(it => it.Id == conversationId);
    }
}
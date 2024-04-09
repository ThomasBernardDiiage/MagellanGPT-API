using Magellan.Api.Services.Interfaces;
using Magellan.DataAccess.Interfaces;
using Magellan.Domain;
using Magellan.Domain.Exceptions;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Magellan.Api.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly Kernel _kernel;
    private readonly IKernelMemory _kernelMemory;
    
    public ConversationService(
        IConversationRepository conversationRepository, 
        Kernel kernel, 
        IKernelMemory kernelMemory
    ) {
        _conversationRepository = conversationRepository;
        _kernel = kernel;
        _kernelMemory = kernelMemory;
    }
    
    
    // This method send an updated value to hub
    // private async Task UpdateQuota(int usedTokens)
    // {
    //     var maxQuota = _configuration.GetValue<int>("MaxQuota");
    //     
    //     var quotaDtoDown = new QuotaDtoDown{CurrentQuota = usedTokens, MaxQuota = maxQuota};
    //     await _messageHub.Clients.All.SendAsync("general", quotaDtoDown);
    // }

    // https://developerscantina.com/p/semantic-kernel-memory/
    // https://github.com/microsoft/kernel-memory/issues/209
    public async Task<MessageEntity> SendMessageInConversation(string userId, Guid conversationId, string message, string model)
    {
        var userMessage = new MessageEntity { Id = Guid.NewGuid(), Sender = MessageSender.User, Text = message, Model = model, Date = DateTime.Now };

        await _conversationRepository.SaveMessageInConversation(userId, conversationId, userMessage);

        var chatHistory = await GetConversationChatHistory(userId, conversationId);
        
        var searchResult = await _kernelMemory.SearchAsync(message);
        
        
        
        if (!searchResult.NoResult)
        {
            var prompt = GetPrompt(searchResult.Results);
            chatHistory.AddSystemMessage(prompt);
        }
    
        var chat = _kernel.Services.GetRequiredKeyedService<IChatCompletionService>(model);
        var result = await chat.GetChatMessageContentAsync(chatHistory, new(), _kernel);
        
        var aiMessage = new MessageEntity { Id = Guid.NewGuid(), Sender = MessageSender.Ai, Text = result.Content ?? "", Model = model, Date = DateTime.Now };
        await _conversationRepository.SaveMessageInConversation(userId, conversationId, aiMessage);

        return aiMessage;
    }

    public async Task<MessageEntity> SendMessageInConversation(string userId, Guid conversationId, string message, List<IFormFile> files, string model)
    {
        var requestGuid = Guid.NewGuid();
        var userMessage = new MessageEntity { Id = Guid.NewGuid(), Sender = MessageSender.User, Text = message, Model = model, Date = DateTime.Now };

        
        await _conversationRepository.SaveMessageInConversation(userId, conversationId, userMessage);

        var chatHistory = await GetConversationChatHistory(userId, conversationId);
        
        var searchResult = await _kernelMemory.SearchAsync(message);
        
        if (!searchResult.NoResult)
        {
            var prompt = GetPrompt(searchResult.Results);
            chatHistory.AddSystemMessage(prompt);
        }


        for (int i = 0; i < files.Count; i++)
            await _kernelMemory.ImportDocumentAsync(files[i].OpenReadStream(), documentId:$"doc{i}-{requestGuid}", fileName:files[i].FileName);
        
        
        MemoryFilter filters = new();
        
        for (int i = 0; i < files.Count; i++)
        {
            filters.ByDocument($"doc{i}-{requestGuid}");
            var imported = await _kernelMemory.IsDocumentReadyAsync($"doc{i}-{requestGuid}");
            if (!imported) throw new DocumentNotImportedException();
        }


        var result = await _kernelMemory.AskAsync(message, filter:filters, minRelevance:0.7);
    
        var aiMessage = new MessageEntity { Id = Guid.NewGuid(), Sender = MessageSender.Ai, Text = result.Result, Model = model, Date = DateTime.Now };
        await _conversationRepository.SaveMessageInConversation(userId, conversationId, aiMessage);

        for (int i = 0; i < files.Count; i++)
            await _kernelMemory.DeleteDocumentAsync($"doc{i}-{requestGuid}");
        
        return aiMessage;
    }

    private string GetPrompt(List<Citation> citations)
    {
        var prompt = """
                     ####
                     The following snippets are fact, they refer to a document uploaded by the user:
                     When using information from one of these snippets, include the snippet number with the <sub>number</sub> in the reply.
                     Example : information used <sub>1</sub>. Another information used <sub>2</sub>.
                     Information from uploaded documents :
                     """;

        for (int i = 0; i < citations.Count; i++)
        {
            prompt += $"""
                       Snippet number {i + 1}, Document title : {citations[i].SourceName}
                       Citations : {citations[i].Partitions.Select(p => p.Text).ToList()}
                       """;
        }

        prompt += "###";

        return prompt;
    }
    
    
    private async Task<ChatHistory> GetConversationChatHistory(string userId, Guid conversationId)
    {
        var chatHistory = new ChatHistory();
                
        var messages = (await _conversationRepository.GetConversationMessages(userId, conversationId)).ToList();
        
        var promptMessages = messages.Where(m => m.Sender == MessageSender.Prompt);
        var nonPromptMessages = messages.Where(m => m.Sender != MessageSender.Prompt)
            .OrderByDescending(m => m.Date) 
            .Take(15); 

        var sortedMessages = promptMessages.Concat(nonPromptMessages)
            .OrderBy(m => m.Date)
            .ToList();
        
        sortedMessages.ForEach(it =>
        {
            switch (it.Sender)
            {
                case MessageSender.User:
                    chatHistory.AddUserMessage(it.Text);
                    break;
                case MessageSender.Ai:
                    chatHistory.AddAssistantMessage(it.Text);
                    break;
                case MessageSender.Prompt:
                    chatHistory.AddSystemMessage(it.Text);
                    break;
            }
        });

        return chatHistory;
    }
}
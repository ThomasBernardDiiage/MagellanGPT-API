namespace Magellan.Api.Dto.Down;

public record ConversationDtoDown(
    Guid Id, 
    string Title, 
    string Prompt,
    DateTime LastModificationDate, 
    IEnumerable<MessageDtoDown> Messages);

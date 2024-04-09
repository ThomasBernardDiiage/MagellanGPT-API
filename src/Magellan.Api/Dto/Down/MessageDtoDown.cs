using DocumentFormat.OpenXml.Office2010.Excel;
using Magellan.Domain;

namespace Magellan.Api.Dto.Down;

public record MessageDtoDown(
    Guid Id,
    MessageSender Sender,
    string Text,
    string Model,
    List<string> FilesNames,
    DateTime Date);
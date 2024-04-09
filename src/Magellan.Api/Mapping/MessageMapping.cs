using AutoMapper;
using Magellan.Api.Dto.Down;
using Magellan.Domain;

namespace Magellan.Api.Mapping;

public class MessageMapping : Profile
{
    public MessageMapping()
    {
        CreateMap<MessageEntity, MessageDtoDown>();
    }
}
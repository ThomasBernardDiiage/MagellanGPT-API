using AutoMapper;
using Magellan.Api.Dto.Down;
using Magellan.Domain;

namespace Magellan.Api.Mapping;

public class ConversationMapping : Profile
{
    public ConversationMapping()
    {
        CreateMap<ConversationEntity, ConversationListDtoDown>();
        
        CreateMap<ConversationEntity, ConversationDtoDown>()
            .ForMember(x => x.Messages, opt => opt.MapFrom(src => src.Messages));
    }
}
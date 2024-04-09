using AutoMapper;
using Magellan.Api.Dto.Down;
using Magellan.Domain;

namespace Magellan.Api.Mapping;

public class ModelMapping : Profile
{
    public ModelMapping()
    {
        CreateMap<ModelEntity, ModelDtoDown>();
    }
}
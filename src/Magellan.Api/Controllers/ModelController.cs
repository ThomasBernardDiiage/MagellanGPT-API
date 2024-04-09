using AutoMapper;
using Magellan.Api.Dto.Down;
using Magellan.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magellan.Api.Controllers;

[Authorize]
[Route("api/models")]
public class ModelController : ControllerBase
{
    private readonly IModelRepository _modelRepository;
    private readonly IMapper _mapper;

    public ModelController(IModelRepository modelRepository, IMapper mapper)
    {
        _modelRepository = modelRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllModel()
    {
        var response = _modelRepository.GetModels();
        return Ok(_mapper.Map<IEnumerable<ModelDtoDown>>(response));
    }
}
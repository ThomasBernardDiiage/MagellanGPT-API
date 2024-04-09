using AutoMapper;
using Magellan.Api.Dto.Down;
using Magellan.Api.Dto.Up;
using Magellan.Api.Services.Interfaces;
using Magellan.DataAccess.Interfaces;
using Magellan.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magellan.Api.Controllers;

// https://medium.com/@akshaykokane09/how-to-build-chatgpt-like-app-with-semantic-kernel-and-azure-cognitive-search-on-internal-data-814e4694decb
[Authorize]
[Route("api/conversations")]
public class MessageController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly ITokenService _tokenService;
    private readonly IModelRepository _modelRepository;
    private readonly IMapper _mapper;

    public MessageController(
        IConversationService conversationService, 
        ITokenService tokenService, 
        IModelRepository modelRepository, 
        IMapper mapper)
    {
        _conversationService = conversationService;
        _tokenService = tokenService;
        _modelRepository = modelRepository;
        _mapper = mapper;
    }
    
    [HttpPost("{id}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessageAsync(Guid id, [FromForm] List<IFormFile> files, [FromForm] SendMessageDtoUp body)
    {
        var modelExist = _modelRepository.ModelExists(body.Model);
        if (!modelExist)
            throw new ModelNotExistsException();
        
        var userId = _tokenService.GetCurrentToken();

        // If files attached, make RAG
        if (files.Any())
        {
            var messageEntity = await _conversationService.SendMessageInConversation(userId, id, body.Message, files, body.Model);
            return Ok(_mapper.Map<MessageDtoDown>(messageEntity));
        }
        else
        {
            var messageEntity = await _conversationService.SendMessageInConversation(userId, id, body.Message, body.Model);
            return Ok(_mapper.Map<MessageDtoDown>(messageEntity));
        }
    }
}
using AutoMapper;
using Magellan.Api.Dto.Down;
using Magellan.Api.Dto.Up;
using Magellan.Api.Services.Interfaces;
using Magellan.DataAccess.Interfaces;
using Magellan.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magellan.Api.Controllers;

[Authorize]
[Route("api/conversations")]
public class ConversationController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;

    public ConversationController(
        IConversationRepository conversationRepository,
        ITokenService tokenService, 
        IMapper mapper)
    {
        _conversationRepository = conversationRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetConversationsAsync()
    {
        var userId = _tokenService.GetCurrentToken();
        var conversations = await _conversationRepository.GetConversationsAsync(userId);
        return Ok(_mapper.Map<IEnumerable<ConversationListDtoDown>>(conversations));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConversation(Guid id)
    {
        var userId = _tokenService.GetCurrentToken();

        if (!await _conversationRepository.ConversationExists(userId, id))
            throw new ConversationNotExistsException();
        
        await _conversationRepository.DeleteConversation(userId, id);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateConversationAsync([FromBody] CreateConversationDtoUp body)
    {
        var userId = _tokenService.GetCurrentToken();
        var conversation = await _conversationRepository.CreateConversation(userId, body.Title, body.Prompt);
        return Created(conversation.Id.ToString(), _mapper.Map<ConversationDtoDown>(conversation));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConversationAsync(Guid id)
    {
        var userId = _tokenService.GetCurrentToken();

        if (!await _conversationRepository.ConversationExists(userId, id))
            throw new ConversationNotExistsException();
        
        var conversations = await _conversationRepository.GetConversation(userId, id);
        return Ok(_mapper.Map<ConversationDtoDown>(conversations));
    }
}
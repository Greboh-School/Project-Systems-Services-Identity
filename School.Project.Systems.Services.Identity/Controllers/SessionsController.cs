using Microsoft.AspNetCore.Mvc;
using School.Project.Systems.Identity.Services;
using School.Project.Systems.Services.Identity.Models.DTOs;
using School.Project.Systems.Services.Identity.Models.Requests;

namespace School.Project.Systems.Identity.Controllers;

// TODO: I generally dont like mixing responsibilities across services / controllers. ex: DI IIdentityService & ITokenService .. Should look into facade pattern
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(IIdentityService identityService, ITokenService tokenService, ILogger<SessionsController> logger)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDTO>> Create([FromBody] SessionCreateRequest request)
    {
         var entity = await _identityService.Authorize(request);
        var accessToken = await _tokenService.Create(entity);
        var dto = new SessionDTO(entity.UserId, entity.UserName!, accessToken);
        
        _logger.LogInformation("{username} with id: {id} successfully logged in!", entity.UserName, entity.Id);

        return Ok(dto);
    }
}
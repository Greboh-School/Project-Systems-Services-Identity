using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Project.Systems.Identity.Services;
using School.Project.Systems.Services.Identity.Models.DTOs;
using School.Project.Systems.Services.Identity.Models.Requests;

namespace School.Project.Systems.Identity.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class IdentitiesController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<IdentitiesController> _logger;

    public IdentitiesController(IIdentityService identityService, ILogger<IdentitiesController> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApplicationUserDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationUserDTO>> Create([FromBody] IdentityCreateRequest request)
    {
        var result = await _identityService.Create(request);
        
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationUserDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize("game:user")]
    public async Task<ActionResult<ApplicationUserDTO>> Get([FromRoute] Guid id)
    {
        var result = await _identityService.Get(id);
        
        return Ok(result);
    }
    
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationUserDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize("game:user")]
    public async Task<ActionResult<ApplicationUserDTO>> GetByUserId([FromRoute] Guid userId)
    {
        var result = await _identityService.GetByUserId(userId);
        
        return Ok(result);
    }
    
}
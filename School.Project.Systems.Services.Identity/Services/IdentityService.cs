using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using School.Project.Systems.Identity.Persistence;
using School.Project.Systems.Services.Identity.Models.DTOs;
using School.Project.Systems.Services.Identity.Models.Entities;
using School.Project.Systems.Services.Identity.Models.Requests;
using School.Shared.Core.Abstractions.Exceptions;
using School.Shared.Core.Authentication.Claims;

namespace School.Project.Systems.Identity.Services;

public interface IIdentityService
{
    public Task<ApplicationUserDTO> Create(IdentityCreateRequest request);
    public Task<ApplicationUserDTO?> Get(Guid id);
    public Task<ApplicationUserDTO?> GetByUserId(Guid userId);
    public Task<ApplicationUser> Authorize(SessionCreateRequest request);
}

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<ApplicationUserDTO> Create(IdentityCreateRequest request)
    {
        var validator = new MinLengthAttribute(6);
        if (string.IsNullOrEmpty(request.UserName) || !validator.IsValid(request.UserName))
        {
            throw new BadRequestException("Invalid username!");
        }

        var entity = new ApplicationUser
        {
            UserName = request.UserName,
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy!
        };

        var result = await _userManager.CreateAsync(entity, request.Password);
        string errorReason;

        if (!result.Succeeded)
        {
            errorReason = result.Errors.First().Description;
            _logger.LogError("Failed to create ApplicationUser with username: {username} because {response}", entity.UserName, errorReason);
            throw new BadRequestException(errorReason);
        }

        result = await AddDefaultClaims(entity);

        if (!result.Succeeded)
        {
            errorReason = result.Errors.First().Description;
            _logger.LogError("Failed while adding default claims, deleting identity with ID: {id} and username {username} because {error}", entity.Id,
                entity.UserName, errorReason);

            await _userManager.DeleteAsync(entity);
            throw new BadRequestException(errorReason);
        }

        _logger.LogInformation("Successfully created identity for {username} with id: {id}", entity.UserName, entity.Id);

        var dto = entity.Adapt<ApplicationUserDTO>();
        
        return dto;
    }

    public async Task<ApplicationUserDTO?> Get(Guid id)
    {
        var entity = await _userManager.FindByIdAsync(id.ToString());

        if (entity is null)
        {
            _logger.LogError("Failed to find user with id {id}", id);
            throw new NotFoundException($"Failed to find user with id {id}");
        }

        var dto = entity.Adapt<ApplicationUserDTO>();

        return dto;
    }

    public async Task<ApplicationUserDTO?> GetByUserId(Guid userId)
    {
        var entity = await _userManager.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        if (entity is null)
        {
            _logger.LogError("Failed to find user with UserId {id}", userId);
            throw new NotFoundException($"Failed to find user with UserId {userId}");
        }

        var dto = entity.Adapt<ApplicationUserDTO>();

        return dto;
    }

    public async Task<ApplicationUser> Authorize(SessionCreateRequest request)
    {
        var entity = await _userManager.FindByNameAsync(request.Username);

        if (entity is null)
        {
            _logger.LogError("Failed to find user: {username}", request.Username);
            throw new NotFoundException($"Failed to find user with username: {request.Username}");
        }

        var signin = await _signInManager.CheckPasswordSignInAsync(entity, request.Password, true);

        // TODO: Customize requirements .. Such as Email confirmation, 2FA option etc

        if (signin.IsLockedOut)
        {
            _logger.LogError("{username} is locked out!", request.Username);
            throw new BadRequestException($"{request.Username} is locked out!");
        }

        if (!signin.Succeeded)
        {
            throw new BadRequestException($"Login failed");
        }

        return entity;
    }

    private async Task<IdentityResult> AddDefaultClaims(ApplicationUser entity)
    {
        var claims = new List<Claim>
        {
            new("iid", entity.Id.ToString()),
            new("uid", entity.UserId.ToString()),
            new(JwtRegisteredClaimNames.Sub, entity.UserName!),
            new("systems:website:role", Enum.GetName(ClaimLevel.User)!),
            new("systems:game:role", Enum.GetName(ClaimLevel.User)!),
        };

        return await _userManager.AddClaimsAsync(entity, claims);
    }
}
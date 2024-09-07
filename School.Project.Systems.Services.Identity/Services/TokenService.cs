using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using School.Project.Systems.Identity.Persistence;
using School.Project.Systems.Services.Identity.Models.Entities;
using School.Shared.Core.Abstractions.Options;
using TokenOptions = School.Project.Systems.Services.Identity.Models.Options.TokenOptions;

namespace School.Project.Systems.Identity.Services;

public interface ITokenService
{
    public Task<string> Create(ApplicationUser user);
}

public class TokenService : ITokenService
{
    private readonly ApplicationUserContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenOptions _tokenOptions;
    private readonly AuthOptions _authOptions;
    private readonly ILogger<TokenService> _logger;

    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenService(ApplicationUserContext context, UserManager<ApplicationUser> userManager, IOptions<TokenOptions> tokenOptions,
        IOptions<AuthOptions> authOptions, ILogger<TokenService> logger)
    {
        _context = context;
        _userManager = userManager;
        _tokenOptions = tokenOptions.Value;
        _authOptions = authOptions.Value;
        _logger = logger;

        _tokenHandler ??= new();
    }

    public async Task<string> Create(ApplicationUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);

        var accessToken = CreateToken(claims);

        return _tokenHandler.WriteToken(accessToken);
    }

    private JwtSecurityToken CreateToken(IList<Claim> claims)
    {
        var expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_tokenOptions.LifeTimeInMinutes));
        const string algorithm = "HS256";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Secret));
        var signingCredentials = new SigningCredentials(key, algorithm);

        return new
        (
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );
    }
}
using Microsoft.AspNetCore.Identity;
using School.Project.Systems.Identity.Persistence;
using School.Project.Systems.Identity.Services;
using School.Project.Systems.Services.Identity.Models.Entities;
using School.Shared.Core.Abstractions;
using School.Shared.Core.Persistence.Extensions;
using TokenOptions = School.Project.Systems.Services.Identity.Models.Options.TokenOptions;

namespace School.Project.Systems.Identity;

public class Configuration : ServiceConfiguration
{
    public override void InjectMiddleware(IApplicationBuilder builder)
    {
        
    }

    public override void InjectServiceRegistrations(IServiceCollection services)
    {
        // Persistence
        services.AddMySQLContext<ApplicationUserContext>("users", Configuration);
        
        // Options
        services.Configure<TokenOptions>(Configuration.GetRequiredSection(TokenOptions.Section));
        
        // Services
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationUserContext>()
            .AddDefaultTokenProviders();
        
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
    }
}
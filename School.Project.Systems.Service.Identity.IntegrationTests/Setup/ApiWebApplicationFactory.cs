using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using School.Project.Systems.Identity;
using School.Project.Systems.Identity.Persistence;
using School.Shared.Core.Abstractions.Options;
using School.Shared.Tools.Test.Extensions;

namespace School.Project.Systems.Service.Identity.IntegrationTests.Setup;

public class ApiWebApplicationFactory : WebApplicationFactory<Configuration>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddTestMySqlContext<ApplicationUserContext>();
            
            services.DisableAuth();
            
            services.Configure<AuthOptions>(options => options.Secret = "This is my test secret, which has more than 256 bytes as is required");
        });
    }
}
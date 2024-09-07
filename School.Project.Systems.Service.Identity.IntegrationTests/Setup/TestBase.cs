using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using School.Project.Systems.Identity.Persistence;
using School.Project.Systems.Services.Identity.Models.Entities;
using Serilog;
using Xunit;

namespace School.Project.Systems.Service.Identity.IntegrationTests.Setup;

[Collection("mysql")]
public class TestBase : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    protected HttpClient Client { get; }
    protected TestServer Server { get; }
    protected ApplicationUserContext Context { get; }
    protected readonly UserManager<ApplicationUser> UserManager;

    
    
    protected readonly IServiceScope Scope;

    
    public TestBase(ApiWebApplicationFactory webApplicationFactory)
    {
        Client = webApplicationFactory.CreateClient();
        Server = webApplicationFactory.Server;
        Scope = webApplicationFactory.Services.CreateScope();
        
        Context = Scope.ServiceProvider.GetRequiredService<ApplicationUserContext>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


        try
        {
            Context.Database.EnsureCreated();
        }
        catch
        {
            // Already exists
        }
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        
        Scope.Dispose();
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}
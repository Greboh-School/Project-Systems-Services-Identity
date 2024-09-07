using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using School.Project.Systems.Service.Identity.IntegrationTests.Setup;
using School.Project.Systems.Services.Identity.Models.DTOs;
using School.Project.Systems.Services.Identity.Models.Entities;
using School.Project.Systems.Services.Identity.Models.Requests;
using School.Shared.Tools.Test.Extensions;
using Xunit;

namespace School.Project.Systems.Service.Identity.IntegrationTests.Controllers;

public class SessionsControllerTests : TestBase
{
    private readonly string _baseAddress = "api/v1/sessions";
    
    public SessionsControllerTests(ApiWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsOkAndCreatesSession()
    {
        // Arrange
        const string username = "Tester";
        const string password = "Test-1234";

        var body = new SessionCreateRequest(username, password);
        
        var entity = new ApplicationUser
        {
            UserName = username,
        };

        await UserManager.CreateAsync(entity, password);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}", body);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.ReadAsAsync<SessionDTO>();
        result.Should().NotBeNull();
        result.Username.Should().BeEquivalentTo(username);
        result.AccessToken.Should().NotBeNullOrEmpty();
    }
}
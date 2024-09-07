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

public class IdentitiesControllerTests : TestBase
{
    private readonly string _baseAddress = "api/v1/identities";
    
    public IdentitiesControllerTests(ApiWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
    }

    [Fact]
    public async Task CreateUser_ValidRequest_ReturnsOkCreatedAndCreatesUser()
    {
        // Arrange
        const string username = "Tester";
        const string password = "Test-1234";
        const string createdBy = "TEST";
        
        var body = new IdentityCreateRequest
        {
            UserName = username,
            Password = password,
            CreatedBy = createdBy
        };

        var expectation = new ApplicationUserDTO(Guid.NewGuid(), Guid.NewGuid(), username);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}", body);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        Context.ApplicationUsers.Should().ContainSingle();
        
        var result = await response.ReadAsAsync<ApplicationUserDTO>();
        result.Should().BeEquivalentTo(expectation, opt =>
            opt.Excluding(x => x.Id)
                .Excluding(x => x.UserId));
    }

    [Fact]
    public async Task Get_ValidRequest_ReturnsDTO()
    {
        // Arrange
        var query = Guid.NewGuid();
        var userName = "Tester";

        var entity = new ApplicationUser
        {
            Id = query,
            UserName = "Tester",
        };

        var expectation = new ApplicationUserDTO(query, Guid.NewGuid(), userName);

        Context.ApplicationUsers.Add(entity);
        await Context.SaveChangesAsync();
        
        // Act
        var response = await Client.GetAsync($"{_baseAddress}/{query}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.ReadAsAsync<ApplicationUserDTO>();
        result.Should().BeEquivalentTo(expectation, opt =>
            opt.Excluding(x => x.UserId));
    }
    
    [Fact]
    public async Task GetByUserId_ValidRequest_ReturnsDTO()
    {
        // Arrange
        var query = Guid.NewGuid();
        var userName = "Tester";

        var entity = new ApplicationUser
        {
            UserId = query,
            UserName = "Tester",
        };

        var expectation = new ApplicationUserDTO(Guid.NewGuid(), query, userName);

        Context.ApplicationUsers.Add(entity);
        await Context.SaveChangesAsync();
        
        // Act
        var response = await Client.GetAsync($"{_baseAddress}/users/{query}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.ReadAsAsync<ApplicationUserDTO>();
        result.Should().BeEquivalentTo(expectation, opt =>
            opt.Excluding(x => x.Id));
    }
}
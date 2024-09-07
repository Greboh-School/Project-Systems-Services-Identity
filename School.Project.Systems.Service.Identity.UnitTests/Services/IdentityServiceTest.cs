using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using School.Project.Systems.Identity.Services;
using School.Project.Systems.Service.Identity.Tests.Setup;
using School.Project.Systems.Services.Identity.Models.Entities;
using School.Project.Systems.Services.Identity.Models.Requests;
using Xunit;

namespace School.Project.Systems.Service.Identity.Tests.Services;

public class IdentityServiceTest : TestBase
{
    private readonly IIdentityService _uut;
    private readonly UserManager<ApplicationUser> _userManagerMock;
    private readonly SignInManager<ApplicationUser> _signInManagerMock;
    private readonly ILogger<IdentityService> _loggerMock;

    public IdentityServiceTest()
    {
        _userManagerMock = Util.CreateUserManagerMock<ApplicationUser>();
        _signInManagerMock = Util.CreateSignInManager(_userManagerMock);
        _loggerMock = Substitute.For<ILogger<IdentityService>>();

        _uut = new IdentityService(_userManagerMock, _signInManagerMock, _loggerMock);
    }

    [Fact]
    public async Task Create_ValidRequest_CreatesUserInDatabase()
    {
        // Arrange
        const string username = "Tester";
        const string password = "Test-1234";
        const string createdBy = "TEST";

        var request = new IdentityCreateRequest
        {
            UserName = username,
            Password = password,
            CreatedBy = createdBy
        };
        var expectation = new ApplicationUser
        {
            UserName = username,
            CreatedBy = createdBy
        };

        // This is necessary because the userManager's CreateAsync adds and calls SaveChanges() but we are mocking it therefor we need to do it
        async void AddToDatabase(CallInfo user)
        {
            await Context.ApplicationUsers.AddAsync(user.Arg<ApplicationUser>());
            await Context.SaveChangesAsync();
        }

        _userManagerMock.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success))
            .AndDoes(AddToDatabase);

        // This is likewise called by CreateAsync.
        _userManagerMock.AddClaimsAsync(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<Claim>>())
            .Returns(Task.FromResult(IdentityResult.Success));

        // Act
        await _uut.Create(request);

        // Assert
        Context.ApplicationUsers.Should().ContainSingle();
        Context.ApplicationUsers
            .First()
            .Should()
            .BeEquivalentTo(expectation, opt =>
                opt.Excluding(x => x.Id)
                    .Excluding(x => x.UserId)
                    .Excluding(x => x.ConcurrencyStamp)
                    .Excluding(x => x.CreatedAt)
                    .ExcludingMissingMembers()
            );
    }
}


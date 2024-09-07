using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace School.Project.Systems.Service.Identity.Tests.Setup;

public static class Util
{
    public static SignInManager<T> CreateSignInManager<T>(UserManager<T> userManager) where T : class
    {
        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<T>>();

        return Substitute.For<SignInManager<T>>(userManager, contextAccessor, claimsFactory, null, null, null, null);
    }

    public static UserManager<T> CreateUserManagerMock<T>() where T : class
    {
        var store = Substitute.For<IUserStore<T>>();
        var manager = Substitute.For<UserManager<T>>(store, null, null, null, null, null, null, null, null);

        return manager;
    }
}
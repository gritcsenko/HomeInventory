using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Web.Modules;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class AuthenticationModuleTestContext : BaseApiModuleGivenTestContext<AuthenticationModuleTestContext, AuthenticationModule>
{
    private readonly IAuthenticationService _authenticationService = Substitute.For<IAuthenticationService>();

    public AuthenticationModuleTestContext(BaseTest test) : base(test) => 
        AddSingleton(_authenticationService);

    public AuthenticationModuleTestContext OnAuthenticationReturn(IVariable<AuthenticateQuery> query, IVariable<AuthenticateResult> result)
    {
        var queryValue = GetValue(query);
        var resultValue = GetValue(result);
        _authenticationService.AuthenticateAsync(queryValue, Arg.Any<CancellationToken>())
            .Returns(Validation<Error, AuthenticateResult>.Success(resultValue));
        return this;
    }

    public AuthenticationModuleTestContext OnAuthenticationReturnError<TError>(IVariable<AuthenticateQuery> query, IVariable<TError> error)
        where TError : Error
    {
        var queryValue = GetValue(query);
        var errorValue = GetValue(error);
        _authenticationService.AuthenticateAsync(queryValue, Arg.Any<CancellationToken>())
            .Returns(Validation<Error, AuthenticateResult>.Fail(errorValue));
        return this;
    }
}

using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class UserManagementModuleTestContext : BaseApiModuleGivenTestContext<UserManagementModuleTestContext, UserManagementCarterModule>
{
    private readonly IRegistrationService _registrationService = Substitute.For<IRegistrationService>();

    public UserManagementModuleTestContext(BaseTest test) : base(test) =>
        AddSingleton(_registrationService);

    public UserManagementModuleTestContext OnRegistrationReturn(IVariable<RegisterCommand> command)
    {
        _registrationService.RegisterAsync(GetValue(command), Arg.Any<CancellationToken>())
            .Returns(Option<Error>.None);
        return this;
    }

    public UserManagementModuleTestContext OnRegistrationReturnError<TError>(IVariable<RegisterCommand> command, IVariable<TError> error)
        where TError : Error
    {
        _registrationService.RegisterAsync(GetValue(command), Arg.Any<CancellationToken>())
            .Returns(Option<Error>.Some(GetValue(error)));
        return this;
    }
}

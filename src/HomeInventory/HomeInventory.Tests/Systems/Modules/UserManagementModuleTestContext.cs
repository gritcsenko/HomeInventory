using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Web.UserManagement;
using HomeInventory.Application.Framework.Messaging;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class UserManagementModuleTestContext : BaseApiModuleGivenTestContext<UserManagementModuleTestContext, UserManagementCarterModule>
{
    private readonly IRegistrationService _registrationService = Substitute.For<IRegistrationService>();
    private readonly ISender _sender = Substitute.For<ISender>();

    public UserManagementModuleTestContext(BaseTest test) : base(test)
    {
        AddSingleton(_registrationService);
        AddSingleton(_sender);
    }

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

    public new UserManagementModuleTestContext OnQueryReturn<TQuery, TResult>(IVariable<TQuery> query, IVariable<TResult> result)
        where TQuery : IQuery<TResult>
        where TResult : notnull
    {
        var queryValue = GetValue(query);
        var resultValue = GetValue(result);
        _sender.Send(queryValue, Arg.Any<CancellationToken>())
            .Returns(new QueryResult<TResult>(resultValue));
        return this;
    }
}

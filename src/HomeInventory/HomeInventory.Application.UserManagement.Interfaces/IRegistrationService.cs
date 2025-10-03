using HomeInventory.Application.UserManagement.Interfaces.Commands;

namespace HomeInventory.Application.UserManagement.Interfaces;

public interface IRegistrationService
{
    Task<Option<Error>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);
}


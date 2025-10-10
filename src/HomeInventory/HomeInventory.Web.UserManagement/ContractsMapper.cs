using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Domain.UserManagement.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace HomeInventory.Web.UserManagement;

[Mapper]
public partial class ContractsMapper
{
    public partial RegisterCommand ToCommand(RegisterRequest contract);

    [MapperIgnoreSource(nameof(RegisterRequest.Password))]
    public partial UserIdQuery ToQuery(RegisterRequest contract);

    public partial RegisterResponse ToResponse(UserIdResult model);

    private static Email ToModel(string email) => new(email);
}


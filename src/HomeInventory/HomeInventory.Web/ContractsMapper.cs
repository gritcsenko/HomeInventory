using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts;
using HomeInventory.Domain.UserManagement.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace HomeInventory.Web;

[Mapper]
public partial class ContractsMapper
{
    public partial AuthenticateQuery ToQuery(LoginRequest contract);

    public partial LoginResponse ToResponse(AuthenticateResult model);

    private static Email ToModel(string email) => new(email);
}


using ErrorOr;
using MediatR;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
public record class AuthenticateQuery(
    string Email,
    string Password
    ) : IRequest<ErrorOr<AuthenticateResult>>;

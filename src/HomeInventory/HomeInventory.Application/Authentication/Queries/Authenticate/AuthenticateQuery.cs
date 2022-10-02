using FluentResults;
using MediatR;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
public record class AuthenticateQuery(
    string Email,
    string Password
    ) : IRequest<Result<AuthenticateResult>>;

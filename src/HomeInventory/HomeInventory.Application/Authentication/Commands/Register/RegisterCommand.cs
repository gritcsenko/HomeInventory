using FluentResults;
using MediatR;

namespace HomeInventory.Application.Authentication.Commands.Register;
public record class RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<Result<RegistrationResult>>;

using ErrorOr;
using MediatR;

namespace HomeInventory.Application.Testing.Commands.ClearDatabase;
public record class ClearDatabaseCommand() : IRequest<ErrorOr<ClearDatabaseResult>>;

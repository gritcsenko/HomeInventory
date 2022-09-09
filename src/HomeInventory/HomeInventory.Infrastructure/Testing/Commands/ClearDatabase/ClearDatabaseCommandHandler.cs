using ErrorOr;
using HomeInventory.Application.Testing.Commands.ClearDatabase;
using HomeInventory.Infrastructure.Persistence;
using MediatR;

namespace HomeInventory.Infrastructure.Testing.Commands.ClearDatabase;
internal class ClearDatabaseCommandHandler : IRequestHandler<ClearDatabaseCommand, ErrorOr<ClearDatabaseResult>>
{
    private readonly DatabaseContext _context;

    public ClearDatabaseCommandHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<ClearDatabaseResult>> Handle(ClearDatabaseCommand request, CancellationToken cancellationToken)
    {
        await _context.Database.EnsureDeletedAsync(cancellationToken);

        return new ClearDatabaseResult();
    }
}

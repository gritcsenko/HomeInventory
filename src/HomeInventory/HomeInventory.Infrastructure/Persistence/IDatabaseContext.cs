using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal interface IDatabaseContext : IDbContext
{
    DbSet<UserModel> Users { get; }
}


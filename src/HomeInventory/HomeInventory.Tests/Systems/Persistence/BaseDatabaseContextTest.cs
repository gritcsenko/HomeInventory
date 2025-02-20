﻿using HomeInventory.Infrastructure.Persistence;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseDatabaseContextTest : BaseTest
{
    private readonly DatabaseContext _context;

    protected BaseDatabaseContextTest()
    {
        _context = DbContextFactory.Default.CreateInMemory<DatabaseContext>(DateTime);
        AddAsyncDisposable(_context);
    }

    private protected DatabaseContext Context => _context;
}

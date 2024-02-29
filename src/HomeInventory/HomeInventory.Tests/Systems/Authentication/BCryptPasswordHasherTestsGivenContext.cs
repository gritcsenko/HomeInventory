﻿using HomeInventory.Infrastructure.Services;

namespace HomeInventory.Tests.Systems.Authentication;

public sealed class BCryptPasswordHasherTestsGivenContext(VariablesContainer variables, IFixture fixture) : GivenContext<BCryptPasswordHasherTestsGivenContext>(variables, fixture)
{
    private readonly Variable<BCryptPasswordHasher> _sut = new(nameof(_sut));

    internal IVariable<BCryptPasswordHasher> Sut => _sut;

    internal BCryptPasswordHasherTestsGivenContext Hasher()
    {
        Add(_sut, () => new BCryptPasswordHasher() { WorkFactor = 6 });
        return this;
    }
}

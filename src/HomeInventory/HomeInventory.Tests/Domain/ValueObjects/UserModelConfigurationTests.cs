﻿using FluentAssertions;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class UserModelConfigurationTests : BaseTest
{
    [Fact]
    public void UserModel_Should_HavePrimaryKey()
    {
        var sut = CreateSut();
        var builder = new ModelBuilder();

        builder.ApplyConfiguration(sut);

        var model = builder.FinalizeModel();
        var type = model.FindRuntimeEntityType(typeof(UserModel));
        type.Should().NotBeNull();
        var primaryKey = type!.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle(x => x.Name == nameof(UserModel.Id));
    }

    private static UserModelConfiguration CreateSut() => new();
}


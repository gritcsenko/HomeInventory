using HomeInventory.Infrastructure.UserManagement.Models;
using HomeInventory.Infrastructure.UserManagement.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
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
        var primaryKey = type.FindPrimaryKey();
        primaryKey.Should().NotBeNull();
        primaryKey.Properties.Should().ContainSingle(static x => x.Name == nameof(UserModel.Id));
    }

    private static UserModelConfiguration CreateSut() => new();
}

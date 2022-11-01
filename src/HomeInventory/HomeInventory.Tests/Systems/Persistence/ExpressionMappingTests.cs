using System.Linq.Expressions;
using FluentAssertions;
using HomeInventory.Domain.Entities;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Systems.Persistence;

public class ExpressionMappingTests : BaseTest
{
    public ExpressionMappingTests()
    {
    }

    [Fact]
    public void Map()
    {
        Expression<Func<User, bool>> source = x => !string.IsNullOrEmpty(x.Email);

        var target = source.Map().To<UserModel>();

        target.Should().NotBeNull();
        target.Compile()(new UserModel { Email = "email" }).Should().BeTrue();
    }
}
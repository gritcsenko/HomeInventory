using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Modules;

public interface IModuleTestGivenContext<TContext>
    where TContext : GivenContext<TContext>, IModuleTestGivenContext<TContext>
{
    TContext Services(out IVariable<IServiceCollection> services, int count = 1, [CallerArgumentExpression(nameof(services))] string? name = null);

    TContext Configuration(out IVariable<IConfiguration> configuration, int count = 1, [CallerArgumentExpression(nameof(configuration))] string? name = null);
}

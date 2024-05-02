using Cake.Core;
using Cake.Frosting;
using Microsoft.Extensions.DependencyInjection;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .ConfigureServices(s => s.AddSingleton<ICakeEnvironment>(Environment))
            .UseContext<Context>()
            .UseLifetime<Lifetime>()
            .UseWorkingDirectory("..")
            .InstallTool(new Uri("dotnet:?package=GitVersion.Tool&version=5.12.0"))
            .InstallTool(new Uri("nuget:?package=Microsoft.TestPlatform&version=" + MicrosoftTestPlatformVersion))
            .InstallTool(new Uri("nuget:?package=NUnit3TestAdapter&version=" + NUnit3TestAdapterVersion))
            .InstallTool(new Uri("nuget:?package=NuGet.CommandLine&version=6.5.0"))
            .Run(args);
    }
}

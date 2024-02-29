using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Format;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO;

namespace Build;

public static class BuildContextExtensions
{
    public static DotNetBuildSettings ToDotNetBuildSettings(this BuildContext context) =>
        new()
        {
            Verbosity = context.Verbosity,
            Configuration = context.BuildConfiguration,
            NoRestore = true,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                ContinuousIntegrationBuild = true,
            }
        };

    public static DotNetFormatSettings ToDotNetFormatSettings(this BuildContext context) =>
        new()
        {
            Verbosity = context.Verbosity,
            VerifyNoChanges = true,
            Severity = DotNetFormatSeverity.Error,
            NoRestore = true,
        };

    public static DotNetRestoreSettings ToDotNetRestoreSettings(this BuildContext context) =>
        new()
        {
            Verbosity = context.Verbosity,
        };

    public static DotNetTestSettings ToDotNetTestSettings(this BuildContext context) =>
        new()
        {
            Verbosity = context.Verbosity,
            Configuration = context.BuildConfiguration,
            Filter = "Category=Unit",
            Loggers = { "trx" },
            Collectors = { "XPlat Code Coverage;CollectCoverage=true;Format=json,lcov,cobertura,opencover;SkipAutoProps=true;IncludeTestAssembly=false;ExcludeByFile=\"**/*.g.cs\"" },
            ResultsDirectory = new DirectoryPath("./coverage")
        };
}

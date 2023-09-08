using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("UnitTest")]
[IsDependentOn(typeof(BuildTask))]
public sealed class UnitTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest(
            context.Tests,
            new DotNetTestSettings
            {
                Filter = "Category=Unit",
                Configuration = context.BuildConfiguration,
                Verbosity = context.Verbosity,
                Loggers = { "trx" },
                Collectors = { "XPlat Code Coverage;CollectCoverage=true;Format=json,lcov,cobertura,opencover;SkipAutoProps=true;IncludeTestAssembly=false;ExcludeByFile=\"**/*.g.cs\"" },
                ResultsDirectory = new DirectoryPath("./coverage")
            });
    }
}
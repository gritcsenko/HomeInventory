using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(
            context.Solution,
            new DotNetBuildSettings
            {
                Configuration = context.BuildConfiguration,
                NoRestore = true,
                Verbosity = context.Verbosity,
                MSBuildSettings = new DotNetMSBuildSettings
                {
                    ContinuousIntegrationBuild = true,
                }
            });
    }
}

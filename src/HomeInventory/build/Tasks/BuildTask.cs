using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<Context>
{
    public override void Run(Context context) => context.DotNetBuild(context.Solution, context.ToDotNetBuildSettings());
}

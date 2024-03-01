using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("UnitTest")]
[IsDependentOn(typeof(BuildTask))]
public sealed class UnitTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context) => context.DotNetTest(context.Tests, context.ToDotNetTestSettings());
}
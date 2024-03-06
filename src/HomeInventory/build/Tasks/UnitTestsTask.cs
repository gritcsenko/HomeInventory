using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("UnitTest")]
[IsDependentOn(typeof(BuildTask))]
public sealed class UnitTestsTask : FrostingTask<Context>
{
    public override void Run(Context context) => context.DotNetTest(context.Tests, context.ToDotNetTestSettings());
}
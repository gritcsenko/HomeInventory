using Cake.Frosting;

namespace Build.Tasks;

[IsDependentOn(typeof(UnitTestsTask))]
public sealed class DefaultTask : FrostingTask<BuildContext>
{
}

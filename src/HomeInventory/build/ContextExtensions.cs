using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;

namespace Build;

public static class ContextExtensions
{
    public static void Git(this ICakeContext context, params string[] arguments)
    {
        var settings = new ProcessSettings();
        foreach (var argument in arguments)
        {
            settings.Arguments.Append(new TextArgument(argument));
        }
        context.Information($"git ${settings.Arguments.Render()}");
        using var process = context.StartAndReturnProcess("git", settings);
        process.WaitForExit();
    }
}

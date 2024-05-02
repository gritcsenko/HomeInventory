using System.Globalization;
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
        var tool = context.Tools.Resolve("git");
        context.RunTool(tool, arguments);
    }

    private static int RunTool(this ICakeContext context, FilePath tool, string[] arguments)
    {
        var settings = CreateProcessSettings(arguments);
        using var process = context.StartProcess(tool, settings);

        WaitForExit(process, tool, settings);
        return process.GetExitCode();
    }

    private static ProcessSettings CreateProcessSettings(string[] arguments)
    {
        var settings = new ProcessSettings();
        foreach (var argument in arguments)
        {
            settings.Arguments.Append(new TextArgument(argument));
        }

        return settings;
    }

    private static IProcess StartProcess(this ICakeContext context, FilePath fileName, ProcessSettings settings)
    {
        context.Information($"{fileName} ${settings.Arguments.Render()}");
        return context.StartAndReturnProcess(fileName, settings);
    }

    private static void WaitForExit(this IProcess process, FilePath tool, ProcessSettings settings)
    {
        if (settings.Timeout is null)
        {
            process.WaitForExit();
            return;
        }

        if (process.WaitForExit(settings.Timeout.Value))
        {
            return;
        }

        throw new TimeoutException(
            string.Format(
                CultureInfo.InvariantCulture,
                "Process TimeOut ({0}): {1}",
                settings.Timeout.Value,
                tool));
    }
}

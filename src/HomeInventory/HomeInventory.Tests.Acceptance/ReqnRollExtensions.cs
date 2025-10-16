namespace HomeInventory.Tests.Acceptance;

internal static class ReqnRollExtensions
{
    internal static void AddAttachmentAsLink(this IReqnrollOutputHelper outputHelper, string path)
    {
        outputHelper.WriteLine($"[Attachment: {path}]");
    }
}


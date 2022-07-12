using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Tests.Helpers;

internal static class ControllerBaseExtensions
{
    public static T WithHttpContext<T>(this T sut)
        where T : ControllerBase
    {
        sut.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };
        return sut;
    }
}

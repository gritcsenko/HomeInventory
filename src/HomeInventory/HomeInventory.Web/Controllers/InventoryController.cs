using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Controllers;

[Route("api/[controller]")]
public class InventoryController : ApiControllerBase
{
    public InventoryController()
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.Zero, cancellationToken);
        return Ok(Array.Empty<string>());
    }
}

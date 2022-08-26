using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Controllers;

[Route("api/[controller]")]
public class InventoryController : ApiControllerBase
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public InventoryController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.Zero, cancellationToken);
        return Ok(Array.Empty<string>());
    }
}

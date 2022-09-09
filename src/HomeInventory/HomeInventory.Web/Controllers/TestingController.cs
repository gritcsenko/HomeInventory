using HomeInventory.Application.Testing.Commands.ClearDatabase;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class TestingController : ApiControllerBase
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public TestingController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("clearDatabase")]
    public async Task<IActionResult> ClearDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ClearDatabaseCommand(), cancellationToken);
        return Match(result, x => Ok());
    }
}

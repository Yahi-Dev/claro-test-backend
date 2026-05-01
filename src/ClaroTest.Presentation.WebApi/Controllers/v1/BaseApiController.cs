using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClaroTest.Presentation.WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??=
        HttpContext.RequestServices.GetRequiredService<IMediator>();
}

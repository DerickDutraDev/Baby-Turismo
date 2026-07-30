using BabyTurismo.Domain.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BabyTurismo.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    
    // Extracted from TenantResolver Middleware
    protected ITenantContext TenantContext => HttpContext.RequestServices.GetRequiredService<ITenantContext>();
}

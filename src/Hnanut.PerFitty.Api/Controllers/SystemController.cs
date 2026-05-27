using Hnanut.PerFitty.SharedKernel.Api;
using Microsoft.AspNetCore.Mvc;

namespace Hnanut.PerFitty.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("version")]
    [ProducesResponseType(typeof(ApiResponse<SystemVersionResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<SystemVersionResponse>> GetVersion()
    {
        var response = new SystemVersionResponse(
            "PerFitty",
            "0.1.0",
            "MVP foundation through T018");

        return Ok(ApiResponse.Success(response));
    }
}

public sealed record SystemVersionResponse(string AppName, string Version, string Milestone);

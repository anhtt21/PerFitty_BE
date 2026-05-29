using System.Security.Claims;
using Hnanut.PerFitty.Application.Features.Auth;
using Hnanut.PerFitty.SharedKernel.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Hnanut.PerFitty.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(
            request,
            CreateClientContext(request.DeviceId, request.DeviceName),
            cancellationToken);

        return ToActionResult(result, StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            request,
            CreateClientContext(request.DeviceId, request.DeviceName),
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(
            request,
            CreateClientContext(request.DeviceId, request.DeviceName),
            cancellationToken);

        return ToActionResult(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<LogoutResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LogoutResponse>>> Logout(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<CurrentUserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CurrentUserResponse>>> Me(
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            var error = new ErrorResponse("invalid_access_token", "Access token is invalid.");
            return Unauthorized(ApiResponse.Failure<CurrentUserResponse>(error));
        }

        var result = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        return ToActionResult(result);
    }

    private AuthClientContext CreateClientContext(string? deviceId, string? deviceName)
    {
        Request.Headers.TryGetValue("User-Agent", out StringValues userAgent);

        return new AuthClientContext(
            deviceId,
            deviceName,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent.ToString());
    }

    private ActionResult<ApiResponse<T>> ToActionResult<T>(
        AuthResult<T> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.Succeeded && result.Value is not null)
        {
            return StatusCode(successStatusCode, ApiResponse.Success(result.Value));
        }

        var failure = result.Error ?? new AuthFailure("auth_error", "Authentication request failed.");
        var error = new ErrorResponse(failure.Code, failure.Message);

        return failure.Code switch
        {
            "email_already_exists" => Conflict(ApiResponse.Failure<T>(error)),
            "invalid_credentials" => Unauthorized(ApiResponse.Failure<T>(error)),
            "invalid_refresh_token" => Unauthorized(ApiResponse.Failure<T>(error)),
            "account_disabled" => Forbid(),
            "database_unavailable" => StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResponse.Failure<T>(error)),
            "user_not_found" => NotFound(ApiResponse.Failure<T>(error)),
            _ => BadRequest(ApiResponse.Failure<T>(error))
        };
    }
}

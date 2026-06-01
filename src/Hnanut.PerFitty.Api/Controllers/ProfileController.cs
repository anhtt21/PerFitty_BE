using System.Security.Claims;
using Hnanut.PerFitty.Application.Features.Profile;
using Hnanut.PerFitty.SharedKernel.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hnanut.PerFitty.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public sealed class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProfileResponse>>> GetProfile(
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return InvalidAccessToken<ProfileResponse>();
        }

        var result = await _profileService.GetProfileAsync(userId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProfileResponse>>> UpdateProfile(
        UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return InvalidAccessToken<ProfileResponse>();
        }

        var result = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("style-preferences")]
    [ProducesResponseType(typeof(ApiResponse<StylePreferencesResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StylePreferencesResponse>>> GetStylePreferences(
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return InvalidAccessToken<StylePreferencesResponse>();
        }

        var result = await _profileService.GetStylePreferencesAsync(userId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPut("style-preferences")]
    [ProducesResponseType(typeof(ApiResponse<StylePreferencesResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StylePreferencesResponse>>> UpdateStylePreferences(
        UpdateStylePreferencesRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return InvalidAccessToken<StylePreferencesResponse>();
        }

        var result = await _profileService.UpdateStylePreferencesAsync(userId, request, cancellationToken);
        return ToActionResult(result);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out userId);
    }

    private ActionResult<ApiResponse<T>> InvalidAccessToken<T>()
    {
        var error = new ErrorResponse("invalid_access_token", "Access token is invalid.");
        return Unauthorized(ApiResponse.Failure<T>(error));
    }

    private ActionResult<ApiResponse<T>> ToActionResult<T>(ProfileResult<T> result)
    {
        if (result.Succeeded && result.Value is not null)
        {
            return Ok(ApiResponse.Success(result.Value));
        }

        var failure = result.Error ?? new ProfileFailure("profile_error", "Profile request failed.");
        var error = new ErrorResponse(failure.Code, failure.Message);

        return failure.Code switch
        {
            "profile_not_found" => NotFound(ApiResponse.Failure<T>(error)),
            "invalid_profile_request" => BadRequest(ApiResponse.Failure<T>(error)),
            "invalid_style_preference" => BadRequest(ApiResponse.Failure<T>(error)),
            _ => BadRequest(ApiResponse.Failure<T>(error))
        };
    }
}
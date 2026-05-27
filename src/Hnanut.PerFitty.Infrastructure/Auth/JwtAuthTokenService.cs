using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Hnanut.PerFitty.Application.Abstractions.Auth;
using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hnanut.PerFitty.Infrastructure.Auth;

public sealed class JwtAuthTokenService : IAuthTokenService
{
    private readonly JwtOptions _options;

    public JwtAuthTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessTokenSecret CreateAccessToken(User user, DateTimeOffset now)
    {
        var expiresAt = now.AddMinutes(_options.AccessTokenMinutes);
        var signingKey = CreateSigningKey();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Email, user.Email),
            new("email_confirmed", user.EmailConfirmed.ToString().ToLowerInvariant())
        };

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            now.UtcDateTime,
            expiresAt.UtcDateTime,
            new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new AccessTokenSecret(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }

    public RefreshTokenSecret CreateRefreshToken(DateTimeOffset now, Guid? tokenFamilyId = null)
    {
        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        var hash = HashRefreshToken(rawToken);

        return new RefreshTokenSecret(
            rawToken,
            hash,
            tokenFamilyId ?? Guid.NewGuid(),
            now.AddDays(_options.RefreshTokenDays));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }

    private SymmetricSecurityKey CreateSigningKey()
    {
        if (string.IsNullOrWhiteSpace(_options.Secret) || _options.Secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret must contain at least 32 characters.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
    }
}
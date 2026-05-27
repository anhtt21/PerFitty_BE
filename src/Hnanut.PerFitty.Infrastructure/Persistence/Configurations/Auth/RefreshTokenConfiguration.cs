using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hnanut.PerFitty.Infrastructure.Persistence.Configurations.Auth;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(token => token.Id)
            .HasName("pk_refresh_tokens");

        builder.Property(token => token.UserId)
            .IsRequired();

        builder.Property(token => token.TokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(token => token.TokenFamilyId)
            .IsRequired();

        builder.Property(token => token.DeviceId)
            .HasMaxLength(128);

        builder.Property(token => token.DeviceName)
            .HasMaxLength(200);

        builder.Property(token => token.IpAddress)
            .HasMaxLength(45);

        builder.Property(token => token.UserAgent)
            .HasMaxLength(512);

        builder.Property(token => token.ExpiresAt)
            .IsRequired();

        builder.Property(token => token.RevokedAt);

        builder.Property(token => token.ReplacedByTokenHash)
            .HasMaxLength(500);

        builder.Property(token => token.CreatedAt)
            .IsRequired();

        builder.Property(token => token.UpdatedAt);

        builder.HasIndex(token => token.TokenHash)
            .IsUnique()
            .HasDatabaseName("ix_refresh_tokens_token_hash");

        builder.HasIndex(token => new { token.UserId, token.TokenFamilyId })
            .HasDatabaseName("ix_refresh_tokens_user_id_token_family_id");

        builder.HasIndex(token => new { token.UserId, token.ExpiresAt })
            .HasDatabaseName("ix_refresh_tokens_user_id_expires_at");
    }
}
using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hnanut.PerFitty.Infrastructure.Persistence.Configurations.Auth;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id)
            .HasName("pk_users");

        builder.Property(user => user.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(user => user.NormalizedEmail)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.EmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(user => user.IsDisabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(user => user.LastLoginAt);

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt);

        builder.HasIndex(user => user.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("ix_users_normalized_email");

        builder.HasOne(user => user.Profile)
            .WithOne(profile => profile.User)
            .HasForeignKey<UserProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_user_profiles_users_user_id");

        builder.HasMany(user => user.RefreshTokens)
            .WithOne(token => token.User)
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_refresh_tokens_users_user_id");

        builder.Navigation(user => user.RefreshTokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
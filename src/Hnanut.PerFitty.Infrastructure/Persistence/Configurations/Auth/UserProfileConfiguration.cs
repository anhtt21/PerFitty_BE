using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hnanut.PerFitty.Infrastructure.Persistence.Configurations.Auth;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(profile => profile.Id)
            .HasName("pk_user_profiles");

        builder.Property(profile => profile.UserId)
            .IsRequired();

        builder.Property(profile => profile.DisplayName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(profile => profile.AvatarObjectKey)
            .HasMaxLength(500);

        builder.Property(profile => profile.Gender)
            .HasMaxLength(30);

        builder.Property(profile => profile.HeightCm)
            .HasColumnType("decimal(5,2)");

        builder.Property(profile => profile.BodyShape)
            .HasMaxLength(60);

        builder.Property(profile => profile.CreatedAt)
            .IsRequired();

        builder.Property(profile => profile.UpdatedAt);

        builder.HasIndex(profile => profile.UserId)
            .IsUnique()
            .HasDatabaseName("ix_user_profiles_user_id");
    }
}
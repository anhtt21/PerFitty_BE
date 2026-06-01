using Hnanut.PerFitty.Domain.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hnanut.PerFitty.Infrastructure.Persistence.Configurations.Auth;

public sealed class UserStylePreferenceConfiguration : IEntityTypeConfiguration<UserStylePreference>
{
    public void Configure(EntityTypeBuilder<UserStylePreference> builder)
    {
        builder.ToTable("user_style_preferences");

        builder.HasKey(preference => preference.Id)
            .HasName("pk_user_style_preferences");

        builder.Property(preference => preference.UserId)
            .IsRequired();

        builder.Property(preference => preference.CreatedAt)
            .IsRequired();

        builder.Property(preference => preference.UpdatedAt);

        builder.HasIndex(preference => preference.UserId)
            .IsUnique()
            .HasDatabaseName("ix_user_style_preferences_user_id");

        builder.HasOne(preference => preference.User)
            .WithOne(user => user.StylePreference)
            .HasForeignKey<UserStylePreference>(preference => preference.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_user_style_preferences_users_user_id");

        builder.HasMany(preference => preference.Values)
            .WithOne(value => value.StylePreference)
            .HasForeignKey(value => value.UserStylePreferenceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_user_style_preference_values_user_style_preferences_id");

        builder.Navigation(preference => preference.Values)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class UserStylePreferenceValueConfiguration : IEntityTypeConfiguration<UserStylePreferenceValue>
{
    public void Configure(EntityTypeBuilder<UserStylePreferenceValue> builder)
    {
        builder.ToTable("user_style_preference_values");

        builder.HasKey(value => value.Id)
            .HasName("pk_user_style_preference_values");

        builder.Property(value => value.UserStylePreferenceId)
            .IsRequired();

        builder.Property(value => value.PreferenceType)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(value => value.Value)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(value => new
        {
            value.UserStylePreferenceId,
            value.PreferenceType,
            value.Value
        })
            .IsUnique()
            .HasDatabaseName("ix_user_style_preference_values_unique_value");
    }
}
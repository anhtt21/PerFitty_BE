using Hnanut.PerFitty.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hnanut.PerFitty.Infrastructure.Persistence.Configurations;

public sealed class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("app_settings");

        builder.HasKey(setting => setting.Id)
            .HasName("pk_app_settings");

        builder.Property(setting => setting.Key)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(setting => setting.Value)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(setting => setting.Description)
            .HasMaxLength(500);

        builder.HasIndex(setting => setting.Key)
            .IsUnique()
            .HasDatabaseName("ix_app_settings_key");
    }
}

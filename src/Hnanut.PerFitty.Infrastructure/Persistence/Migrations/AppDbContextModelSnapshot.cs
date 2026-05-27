using Hnanut.PerFitty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Hnanut.PerFitty.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
sealed partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.7")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Hnanut.PerFitty.Infrastructure.Persistence.Entities.AppSetting", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uniqueidentifier")
                .HasColumnName("id");

            b.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("datetimeoffset")
                .HasColumnName("created_at");

            b.Property<string>("Description")
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)")
                .HasColumnName("description");

            b.Property<string>("Key")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("nvarchar(120)")
                .HasColumnName("key");

            b.Property<DateTimeOffset?>("UpdatedAt")
                .HasColumnType("datetimeoffset")
                .HasColumnName("updated_at");

            b.Property<string>("Value")
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnType("nvarchar(2000)")
                .HasColumnName("value");

            b.HasKey("Id")
                .HasName("pk_app_settings");

            b.HasIndex("Key")
                .IsUnique()
                .HasDatabaseName("ix_app_settings_key");

            b.ToTable("app_settings", (string)null);
        });
#pragma warning restore 612, 618
    }
}

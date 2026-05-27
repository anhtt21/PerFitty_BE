using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hnanut.PerFitty.Infrastructure.Persistence.Migrations;

public partial class InitialDatabaseFoundation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "app_settings",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                key = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_app_settings", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_app_settings_key",
            table: "app_settings",
            column: "key",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "app_settings");
    }
}

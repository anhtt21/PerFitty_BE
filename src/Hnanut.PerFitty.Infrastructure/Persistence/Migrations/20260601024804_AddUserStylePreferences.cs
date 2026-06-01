using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hnanut.PerFitty.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStylePreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_style_preferences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_style_preferences", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_style_preferences_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_style_preference_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_style_preference_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    preference_type = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    value = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_style_preference_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_style_preference_values_user_style_preferences_id",
                        column: x => x.user_style_preference_id,
                        principalTable: "user_style_preferences",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_style_preference_values_unique_value",
                table: "user_style_preference_values",
                columns: new[] { "user_style_preference_id", "preference_type", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_style_preferences_user_id",
                table: "user_style_preferences",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_style_preference_values");

            migrationBuilder.DropTable(
                name: "user_style_preferences");
        }
    }
}

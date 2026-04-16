using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebhookForge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiProviderSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnthropicApiKey",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AiApiKey",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiProvider",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiApiKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AiProvider",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AnthropicApiKey",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

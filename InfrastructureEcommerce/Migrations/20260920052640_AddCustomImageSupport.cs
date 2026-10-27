using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomImageSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomizable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CustomImageUrl",
                table: "OrderItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomizable",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomImageUrl",
                table: "OrderItems");
        }
    }
}

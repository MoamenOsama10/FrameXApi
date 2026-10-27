using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class FinalPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepositAccount",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositReceiptUrl",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositAccount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DepositReceiptUrl",
                table: "Orders");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class paymode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentModeID",
                table: "Expense");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMode",
                table: "Expense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Expense");

            migrationBuilder.AddColumn<int>(
                name: "PaymentModeID",
                table: "Expense",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

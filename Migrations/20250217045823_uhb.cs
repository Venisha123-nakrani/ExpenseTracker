using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class uhb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Payment_PaymentModeID",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Payment_PaymentModeID1",
                table: "Incomes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "IncomeCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Payments_PaymentModeID",
                table: "Expenses",
                column: "PaymentModeID",
                principalTable: "Payments",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Payments_PaymentModeID1",
                table: "Incomes",
                column: "PaymentModeID1",
                principalTable: "Payments",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Payments_PaymentModeID",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Payments_PaymentModeID1",
                table: "Incomes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "IncomeCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "PaymentModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Payment_PaymentModeID",
                table: "Expenses",
                column: "PaymentModeID",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Payment_PaymentModeID1",
                table: "Incomes",
                column: "PaymentModeID1",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

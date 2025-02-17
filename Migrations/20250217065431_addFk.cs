using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class addFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_Payment_PaymentModeID",
                table: "Expense");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_Payment_PaymentModeID",
                table: "Expense",
                column: "PaymentModeID",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_Payment_PaymentModeID",
                table: "Expense");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_Payment_PaymentModeID",
                table: "Expense",
                column: "PaymentModeID",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

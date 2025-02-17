using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class qwer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Payments_PaymentModeID1",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_PaymentModeID1",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "PaymentModeID1",
                table: "Incomes");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_PaymentModeID",
                table: "Incomes",
                column: "PaymentModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Payments_PaymentModeID",
                table: "Incomes",
                column: "PaymentModeID",
                principalTable: "Payments",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Payments_PaymentModeID",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_PaymentModeID",
                table: "Incomes");

            migrationBuilder.AddColumn<int>(
                name: "PaymentModeID1",
                table: "Incomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_PaymentModeID1",
                table: "Incomes",
                column: "PaymentModeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Payments_PaymentModeID1",
                table: "Incomes",
                column: "PaymentModeID1",
                principalTable: "Payments",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

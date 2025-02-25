using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class htg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Payments_PaymentModeID",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaymentModeID",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "PaymentModeID1",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentModeID1",
                table: "Expenses",
                column: "PaymentModeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Payments_PaymentModeID1",
                table: "Expenses",
                column: "PaymentModeID1",
                principalTable: "Payments",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Payments_PaymentModeID1",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaymentModeID1",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaymentModeID1",
                table: "Expenses");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentModeID",
                table: "Expenses",
                column: "PaymentModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Payments_PaymentModeID",
                table: "Expenses",
                column: "PaymentModeID",
                principalTable: "Payments",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

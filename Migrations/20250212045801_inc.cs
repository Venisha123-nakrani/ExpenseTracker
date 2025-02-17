using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class inc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Income_IncomeCategory_IncomeCategoryID",
                table: "Income");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_Payment_PaymentModeID",
                table: "Income");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_Users_UserID",
                table: "Income");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomeCategory",
                table: "IncomeCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Income",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Income_PaymentModeID",
                table: "Income");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Income");

            migrationBuilder.RenameTable(
                name: "IncomeCategory",
                newName: "IncomeCategories");

            migrationBuilder.RenameTable(
                name: "Income",
                newName: "Incomes");

            migrationBuilder.RenameIndex(
                name: "IX_Income_UserID",
                table: "Incomes",
                newName: "IX_Incomes_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Income_IncomeCategoryID",
                table: "Incomes",
                newName: "IX_Incomes_IncomeCategoryID");

            migrationBuilder.AddColumn<int>(
                name: "PaymentModeID1",
                table: "Incomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomeCategories",
                table: "IncomeCategories",
                column: "IncomeCategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Incomes",
                table: "Incomes",
                column: "IncomeID");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_PaymentModeID1",
                table: "Incomes",
                column: "PaymentModeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_IncomeCategories_IncomeCategoryID",
                table: "Incomes",
                column: "IncomeCategoryID",
                principalTable: "IncomeCategories",
                principalColumn: "IncomeCategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Payment_PaymentModeID1",
                table: "Incomes",
                column: "PaymentModeID1",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Users_UserID",
                table: "Incomes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_IncomeCategories_IncomeCategoryID",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Payment_PaymentModeID1",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Users_UserID",
                table: "Incomes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Incomes",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_PaymentModeID1",
                table: "Incomes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomeCategories",
                table: "IncomeCategories");

            migrationBuilder.DropColumn(
                name: "PaymentModeID1",
                table: "Incomes");

            migrationBuilder.RenameTable(
                name: "Incomes",
                newName: "Income");

            migrationBuilder.RenameTable(
                name: "IncomeCategories",
                newName: "IncomeCategory");

            migrationBuilder.RenameIndex(
                name: "IX_Incomes_UserID",
                table: "Income",
                newName: "IX_Income_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Incomes_IncomeCategoryID",
                table: "Income",
                newName: "IX_Income_IncomeCategoryID");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMode",
                table: "Income",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Income",
                table: "Income",
                column: "IncomeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomeCategory",
                table: "IncomeCategory",
                column: "IncomeCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Income_PaymentModeID",
                table: "Income",
                column: "PaymentModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Income_IncomeCategory_IncomeCategoryID",
                table: "Income",
                column: "IncomeCategoryID",
                principalTable: "IncomeCategory",
                principalColumn: "IncomeCategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_Payment_PaymentModeID",
                table: "Income",
                column: "PaymentModeID",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_Users_UserID",
                table: "Income",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

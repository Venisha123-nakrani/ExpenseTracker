using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class dbpriupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "User",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "RecurringExpense",
                newName: "RecurringExpenseID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ExpenseReport",
                newName: "ExpenseReportID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ExpenseCategory",
                newName: "ExpenseCategoryID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Expense",
                newName: "ExpenseID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Budget",
                newName: "BudgetID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Attachment",
                newName: "AttachmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "User",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "RecurringExpenseID",
                table: "RecurringExpense",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ExpenseReportID",
                table: "ExpenseReport",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ExpenseCategoryID",
                table: "ExpenseCategory",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ExpenseID",
                table: "Expense",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "BudgetID",
                table: "Budget",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "AttachmentID",
                table: "Attachment",
                newName: "ID");
        }
    }
}

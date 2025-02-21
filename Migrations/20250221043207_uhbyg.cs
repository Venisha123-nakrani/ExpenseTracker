using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class uhbyg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_CategoryID",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Expenses",
                newName: "ExpenseCategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_CategoryID",
                table: "Expenses",
                newName: "IX_Expenses_ExpenseCategoryID");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "ExpenseCategories",
                newName: "ExpenseCategoryID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpenseDate",
                table: "Expenses",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Expenses",
                type: "datetime2(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ExpenseCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryID",
                table: "Expenses",
                column: "ExpenseCategoryID",
                principalTable: "ExpenseCategories",
                principalColumn: "ExpenseCategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryID",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "ExpenseCategoryID",
                table: "Expenses",
                newName: "CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_ExpenseCategoryID",
                table: "Expenses",
                newName: "IX_Expenses_CategoryID");

            migrationBuilder.RenameColumn(
                name: "ExpenseCategoryID",
                table: "ExpenseCategories",
                newName: "CategoryID");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpenseDate",
                table: "Expenses",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Expenses",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ExpenseCategories",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_CategoryID",
                table: "Expenses",
                column: "CategoryID",
                principalTable: "ExpenseCategories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

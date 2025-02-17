using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class incomepaymenttbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIncome",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "PaymentModeID",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IncomeCategory",
                columns: table => new
                {
                    IncomeCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeCategory", x => x.IncomeCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentModeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentModeID);
                });

            migrationBuilder.CreateTable(
                name: "Income",
                columns: table => new
                {
                    IncomeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    IncomeCategoryID = table.Column<int>(type: "int", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncomeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentModeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Income", x => x.IncomeID);
                    table.ForeignKey(
                        name: "FK_Income_IncomeCategory_IncomeCategoryID",
                        column: x => x.IncomeCategoryID,
                        principalTable: "IncomeCategory",
                        principalColumn: "IncomeCategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Income_Payment_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "Payment",
                        principalColumn: "PaymentModeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Income_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentModeID",
                table: "Expenses",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Income_IncomeCategoryID",
                table: "Income",
                column: "IncomeCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Income_PaymentModeID",
                table: "Income",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Income_UserID",
                table: "Income",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Payment_PaymentModeID",
                table: "Expenses",
                column: "PaymentModeID",
                principalTable: "Payment",
                principalColumn: "PaymentModeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Payment_PaymentModeID",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "Income");

            migrationBuilder.DropTable(
                name: "IncomeCategory");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaymentModeID",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaymentModeID",
                table: "Expenses");

            migrationBuilder.AddColumn<bool>(
                name: "IsIncome",
                table: "Expenses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMode",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

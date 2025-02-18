﻿// <auto-generated />
using System;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ExpenseTracker.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ExpenseTracker.Model.Attachment", b =>
                {
                    b.Property<int>("AttachmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AttachmentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttachmentID"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<int>("ExpenseID")
                        .HasColumnType("int")
                        .HasColumnName("ExpenseID");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.HasKey("AttachmentID");

                    b.HasIndex("ExpenseID");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Budget", b =>
                {
                    b.Property<int>("BudgetID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BudgetID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BudgetID"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("CategoryID")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("BudgetID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("UserID");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Expense", b =>
                {
                    b.Property<int>("ExpenseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ExpenseID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpenseID"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("CategoryID")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateOnly>("ExpenseDate")
                        .HasColumnType("date");

                    b.Property<int>("PaymentModeID")
                        .HasColumnType("int")
                        .HasColumnName("PaymentModeID");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ExpenseID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("PaymentModeID");

                    b.HasIndex("UserID");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("ExpenseTracker.Model.ExpenseCategory", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryID"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("CategoryID");

                    b.ToTable("ExpenseCategories");
                });

            modelBuilder.Entity("ExpenseTracker.Model.ExpenseReport", b =>
                {
                    b.Property<int>("ReportID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ReportID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReportID"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ReportID");

                    b.HasIndex("UserID");

                    b.ToTable("ExpenseReports");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Income", b =>
                {
                    b.Property<int>("IncomeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IncomeID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IncomeID"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("IncomeCategoryID")
                        .HasColumnType("int")
                        .HasColumnName("IncomeCategoryID");

                    b.Property<DateTime>("IncomeDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PaymentModeID")
                        .HasColumnType("int")
                        .HasColumnName("PaymentModeID");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("IncomeID");

                    b.HasIndex("IncomeCategoryID");

                    b.HasIndex("PaymentModeID");

                    b.HasIndex("UserID");

                    b.ToTable("Incomes");
                });

            modelBuilder.Entity("ExpenseTracker.Model.IncomeCategory", b =>
                {
                    b.Property<int>("IncomeCategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IncomeCategoryID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IncomeCategoryID"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IncomeCategoryID");

                    b.ToTable("IncomeCategories");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Payment", b =>
                {
                    b.Property<int>("PaymentModeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PaymentModeID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentModeID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("PaymentModeID");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("ExpenseTracker.Model.RecurringExpense", b =>
                {
                    b.Property<int>("RecurringID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RecurringID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecurringID"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("CategoryID")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Frequency")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("RecurringID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("UserID");

                    b.ToTable("RecurringExpenses");
                });

            modelBuilder.Entity("ExpenseTracker.Model.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserID");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserID = 1,
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "admin@expense.com",
                            FullName = "Admin User",
                            PasswordHash = "Admin@123"
                        },
                        new
                        {
                            UserID = 2,
                            CreatedAt = new DateTime(2024, 1, 2, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "johndoe@example.com",
                            FullName = "John Doe",
                            PasswordHash = "User@123"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("ExpenseTracker.Model.Attachment", b =>
                {
                    b.HasOne("ExpenseTracker.Model.Expense", "Expense")
                        .WithMany("Attachments")
                        .HasForeignKey("ExpenseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expense");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Budget", b =>
                {
                    b.HasOne("ExpenseTracker.Model.ExpenseCategory", "Category")
                        .WithMany("Budgets")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpenseTracker.Model.User", "User")
                        .WithMany("Budgets")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Expense", b =>
                {
                    b.HasOne("ExpenseTracker.Model.ExpenseCategory", "Category")
                        .WithMany("Expenses")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpenseTracker.Model.Payment", "Payment")
                        .WithMany("Expenses")
                        .HasForeignKey("PaymentModeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ExpenseTracker.Model.User", "User")
                        .WithMany("Expenses")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Payment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ExpenseTracker.Model.ExpenseReport", b =>
                {
                    b.HasOne("ExpenseTracker.Model.User", "User")
                        .WithMany("ExpenseReports")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Income", b =>
                {
                    b.HasOne("ExpenseTracker.Model.IncomeCategory", "IncomeCategory")
                        .WithMany("Incomes")
                        .HasForeignKey("IncomeCategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpenseTracker.Model.Payment", "Payment")
                        .WithMany("Incomes")
                        .HasForeignKey("PaymentModeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ExpenseTracker.Model.User", "User")
                        .WithMany("Incomes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IncomeCategory");

                    b.Navigation("Payment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ExpenseTracker.Model.RecurringExpense", b =>
                {
                    b.HasOne("ExpenseTracker.Model.ExpenseCategory", "Category")
                        .WithMany("RecurringExpenses")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpenseTracker.Model.User", "User")
                        .WithMany("RecurringExpenses")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ExpenseTracker.Model.Expense", b =>
                {
                    b.Navigation("Attachments");
                });

            modelBuilder.Entity("ExpenseTracker.Model.ExpenseCategory", b =>
                {
                    b.Navigation("Budgets");

                    b.Navigation("Expenses");

                    b.Navigation("RecurringExpenses");
                });

            modelBuilder.Entity("ExpenseTracker.Model.IncomeCategory", b =>
                {
                    b.Navigation("Incomes");
                });

            modelBuilder.Entity("ExpenseTracker.Model.Payment", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("Incomes");
                });

            modelBuilder.Entity("ExpenseTracker.Model.User", b =>
                {
                    b.Navigation("Budgets");

                    b.Navigation("ExpenseReports");

                    b.Navigation("Expenses");

                    b.Navigation("Incomes");

                    b.Navigation("RecurringExpenses");
                });
#pragma warning restore 612, 618
        }
    }
}

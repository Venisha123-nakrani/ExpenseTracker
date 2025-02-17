using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Model;

public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string FullName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [NotMapped]  // This field is used only for form binding, not stored in DB
    public string Password { get; set; } = null!;


    [Column(TypeName = "text")]
    public string PasswordHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [InverseProperty("User")]
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    [InverseProperty("User")]
    public virtual ICollection<ExpenseReport> ExpenseReports { get; set; } = new List<ExpenseReport>();

    [InverseProperty("User")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    [InverseProperty("User")]
    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    [InverseProperty("User")]
    public virtual ICollection<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();
}

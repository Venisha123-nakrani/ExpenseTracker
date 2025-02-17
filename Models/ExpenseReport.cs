using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models;

public partial class ExpenseReport
{
    [Key]
    [Column("ReportID")]
    public int ReportID { get; set; }

    [Column("UserID")]
    public int UserID { get; set; }
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("ExpenseReports")]
    public virtual User User { get; set; } = null!;
}

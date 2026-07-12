using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Data;

public class Expense
{
    [Key]
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public double Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    public string UserId { get; set; } = string.Empty;
}

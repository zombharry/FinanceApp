using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public double Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;
}

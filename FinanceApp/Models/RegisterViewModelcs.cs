using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Models;

public class RegisterViewModelcs
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }
}

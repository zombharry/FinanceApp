using System.ComponentModel.DataAnnotations;

namespace Api.Client.Client.DTOs;

public class LoginRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

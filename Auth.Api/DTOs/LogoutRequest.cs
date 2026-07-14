using System.ComponentModel.DataAnnotations;

namespace Auth.Api.DTOs;

public class LogoutRequest
{
    [Required]
    public string Username { get; set; }
}

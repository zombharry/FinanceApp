using System.ComponentModel.DataAnnotations;

namespace Api.Shared.Authentication.DTOs;

public class LogoutRequest
{
    [Required]
    public string Username { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Api.Client.Client.DTOs;

public class LogoutRequest
{
    [Required]
    public string Username { get; set; }
}

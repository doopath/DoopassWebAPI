using System.ComponentModel.DataAnnotations;

namespace Doopass.API.Domain.DTOs;

public record UserDTO
{
    [MaxLength(255)]
    public string? UserName { get; set; }

    public string? Password { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Doopass.API.Domain.DTOs;

public record UserDTO
{
    [MaxLength(255)] public string? UserName { get; init; }

    public string? Password { get; init; }

    [EmailAddress] public string? Email { get; init; }
}
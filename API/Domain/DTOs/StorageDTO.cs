using System.ComponentModel.DataAnnotations;

namespace Doopass.API.Domain.DTOs;

public record StorageDTO
{
    public int Id { get; set; }

    [MaxLength(255)] public string? Name { get; set; }

    public string? Content { get; set; }

    public int UserId { get; set; }
}
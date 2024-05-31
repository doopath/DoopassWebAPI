using System.ComponentModel.DataAnnotations;

namespace Doopass.API.Models;

public class Storage
{
    public int Id { get; set; }

    [MaxLength(255)]
    public required string Name { get; set; }

    public required string Content { get; set; }

    public int UserId { get; set; }

    public required User User { get; set; }
}
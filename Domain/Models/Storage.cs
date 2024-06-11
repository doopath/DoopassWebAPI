using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class Storage
{
    public int Id { get; set; }
    [MaxLength(255)] public required string Name { get; set; }
    public required string Content { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")] public required User User { get; set; }
}
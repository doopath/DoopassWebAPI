using System.ComponentModel.DataAnnotations;

namespace Doopass.API.Domain.Models;

public class User
{
    public int Id { get; set; }

    [MaxLength(255)]
    public required string UserName { get; set; }

    public required string Password { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
}
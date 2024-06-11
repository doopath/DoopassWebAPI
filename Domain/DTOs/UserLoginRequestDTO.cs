namespace Domain.DTOs;

public record UserLoginRequestDTO
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
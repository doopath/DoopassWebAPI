namespace Domain.DTOs;

public record JWTTokenDTO
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
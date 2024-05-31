namespace Doopass.API.Domain.DTOs;

public record JWTTokenDTO
{
    public required string AccessToken { get; init; }
    public required string UserName { get; init; }
}
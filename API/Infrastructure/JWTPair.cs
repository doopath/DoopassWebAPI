namespace Doopass.API.Infrastructure;

public record JWTPair
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
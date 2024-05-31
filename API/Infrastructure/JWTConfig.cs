namespace Doopass.API.Infrastructure;

public record JWTConfig(string issuer, string audience, double lifeTime, string secretKeyVariableName)
{
    public string Issuer { get; init; } = issuer;
    public string Audience { get; init; } = audience;
    public double LifeTime { get; init; } = lifeTime;
    public string SecretKey { get; init; } = Environment.GetEnvironmentVariable(secretKeyVariableName)!;
}
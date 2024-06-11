namespace Infrastructure;

public record JWTConfig(
    string issuer,
    string audience,
    double accessLifetime,
    double refreshLifetime,
    string secretKeyVariableName)
{
    public string Issuer { get; init; } = issuer;
    public string Audience { get; init; } = audience;
    public double AccessLifetime { get; init; } = accessLifetime;
    public double RefreshLifetime { get; init; } = refreshLifetime;
    public string SecretKey { get; init; } = Environment.GetEnvironmentVariable(secretKeyVariableName)!;
}
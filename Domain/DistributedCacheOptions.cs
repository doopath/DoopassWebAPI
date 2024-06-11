using Domain.Contracts;

namespace Domain;

public class DistributedCacheOptions : IDbOptions
{
    private const string PasswordVar = "REDIS_PASSWORD";
    
    private string Password { get; set; } = Environment.GetEnvironmentVariable(PasswordVar)!;

    public string ConnectionString => $"0.0.0.0:6379,password={Password}";
}
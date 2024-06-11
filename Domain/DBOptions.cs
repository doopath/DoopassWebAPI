using Domain.Contracts;

namespace Domain;

public class DbOptions : IDbOptions
{
    private string User { get; init; }
    private string Password { get; init; }
    private string Database { get; init; }
    private string Host { get; init; }
    private string Port { get; init; }

    private const string UserVar = "DOOPASS_API_USER";
    private const string PasswordVar = "DOOPASS_API_PASSWORD";
    private const string DatabaseVar = "DOOPASS_API_DATABASE";
    private const string HostVar = "DOOPASS_API_HOST";
    private const string PortVar = "DOOPASS_API_PORT";

    public string ConnectionString =>
        $"User ID={User};Password={Password};Host={Host};Port={Port};Database={Database};";

    public DbOptions()
    {
        User = Environment.GetEnvironmentVariable(UserVar)!;
        Password = Environment.GetEnvironmentVariable(PasswordVar)!;
        Database = Environment.GetEnvironmentVariable(DatabaseVar)!;
        Host = Environment.GetEnvironmentVariable(HostVar)!;
        Port = Environment.GetEnvironmentVariable(PortVar)!;
    }
}
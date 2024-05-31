namespace Doopass.API.Domain.Models;

public class DBOptions
{
    public string User { get; init;}
    public string Password { get; init;}
    public string Database { get; init;}
    public string Host { get; init;}
    public string Port { get; init;}

    private readonly string _UserVar = "DOOPASS_API_USER";
    private readonly string _PasswordVar = "DOOPASS_API_PASSWORD";
    private readonly string _DatabaseVar = "DOOPASS_API_DATABASE";
    private readonly string _HostVar = "DOOPASS_API_HOST";
    private readonly string _PortVar = "DOOPASS_API_PORT";

    public string ConnectionString { get {
        return $"User ID={User};Password={Password};Host={Host};Port={Port};Database={Database};";
    }}

    public DBOptions()
    {
        User = Environment.GetEnvironmentVariable(_UserVar)!;
        Password = Environment.GetEnvironmentVariable(_PasswordVar)!;
        Database = Environment.GetEnvironmentVariable(_DatabaseVar)!;
        Host = Environment.GetEnvironmentVariable(_HostVar)!;
        Port = Environment.GetEnvironmentVariable(_PortVar)!;
    }
}
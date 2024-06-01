namespace Doopass.API.Infrastructure;

public interface IPasswordValidator
{
    public string Password { get; init; }
    public bool Validate();
}
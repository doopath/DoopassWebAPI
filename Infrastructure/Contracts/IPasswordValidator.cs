namespace Infrastructure.Contracts;

public interface IPasswordValidator
{
    public string Password { get; init; }
    public bool Validate();
}
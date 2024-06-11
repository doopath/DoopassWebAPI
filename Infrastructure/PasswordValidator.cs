using System.Text.RegularExpressions;
using Infrastructure.Contracts;

namespace Infrastructure;

public class PasswordValidator(string password) : IPasswordValidator
{
    public string Password { get; init; } = password;

    public bool Validate()
    {
        Regex r = new(@"(?=.{8,})(?=(.*\d){1,})(?=(.*\W){1,})");
        
        return r.IsMatch(Password);
    }
}
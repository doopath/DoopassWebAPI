using Doopass.API.Domain.Models;

namespace Doopass.API.Domain.DTOs;

public static class UserExtensions
{
    public static UserDTO ToDTO(this User user)
    {
        return new()
        {
            UserName = user.UserName,
            Email = user.Email,
            Password = user.Password
        };
    }
}
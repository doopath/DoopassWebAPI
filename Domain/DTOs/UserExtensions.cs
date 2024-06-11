using System.Text;
using System.Text.Json;
using Domain.Models;

namespace Domain.DTOs;

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

    public static string ToCacheKey(this UserDTO userDTO)
    {
        var serialized = JsonSerializer.Serialize(userDTO.UserName);
        var bytes = Encoding.UTF8.GetBytes(serialized);
        
        return Convert.ToBase64String(bytes);
    }
}
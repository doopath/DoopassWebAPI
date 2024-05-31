using Doopass.API.Domain.DTOs;
using Doopass.API.Infrastructure;
using Doopass.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Doopass.API.Infrastructure;

public interface IUserService
{
    public Task<User?> GetUser(string username);
    public Task<List<User>> GetUsers();
    public Task<User> UpdateUser(UserDTO user);
    public void DeleteUser(string username);
    public Task<JWTPair> Login(UserLoginRequestDTO data);
    public Task<JWTPair> Refresh(string username);
    public Task<User> Register(UserDTO userDTO);
}
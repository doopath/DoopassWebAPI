using Domain.DTOs;
using Domain.Models;

namespace Infrastructure.Contracts;

public interface IUserService
{
    public Task<User?> GetUser(string username);
    public Task<List<User>> GetUsers();
    public Task<User> UpdateUser(UserDTO user);
    public Task DeleteUser(string username);
    public Task<JWTPair> Login(UserLoginRequestDTO data);
    public Task<JWTPair> Refresh(string username);
    public Task<User> Register(UserDTO userDTO);
}
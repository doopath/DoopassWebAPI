using Doopass.API.Domain.DTOs;
using Doopass.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Doopass.API.Infrastructure.Services;

public class UserService : IUserService
{
    protected DoopassContext _dbContext;
    protected JWTProvider _jwtProvider;

    public UserService(DoopassContext dbContext, JWTConfig jwtConfig)
    {
        _dbContext = dbContext;
        _jwtProvider = new JWTProvider(jwtConfig, new PasswordHasher(), this);
    }

    public async Task<User?> GetUser(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);
    }

    public async Task<List<User>> GetUsers()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User> UpdateUser(UserDTO userDTO)
    {
        User user = await _dbContext.Users.FirstAsync(user => user.UserName == userDTO.UserName);
        user.UserName = userDTO.UserName ?? user.UserName;
        user.Email = userDTO.Email ?? user.Email;

        if (userDTO.Password is not null)
            user.Password = new PasswordHasher().Generate(userDTO.Password);

        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async void DeleteUser(string username)
    {
        User user = await _dbContext.Users.FirstAsync(user => user.UserName == username);
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> Register(UserDTO userDTO)
    {
        User user = new() {
            UserName = userDTO.UserName!,
            Password = new PasswordHasher().Generate(userDTO.Password!),
            Email = userDTO.Email!,
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<string> Login(UserLoginRequestDTO data)
    {
        return await _jwtProvider.Generate(data.UserName);
    }
}
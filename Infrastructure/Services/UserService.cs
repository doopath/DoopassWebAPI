using Domain;
using Domain.DTOs;
using Domain.Models;
using Infrastructure.Contracts;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly DoopassContext _dbContext;
    private readonly JWTProvider _jwtProvider;

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
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == userDTO.UserName);
        
        if (user is null)
            throw new EntityNotFoundException($"User with username={userDTO.UserName} was not found");
        
        user.Email = userDTO.Email ?? user.Email;

        if (userDTO.Password is not null)
        {
            if (new PasswordValidator(userDTO.Password).Validate() is false)
                throw new InvalidPasswordException(
                    "Password must be at least 8 symbols and contain one digit and one special character!");
            user.Password = new PasswordHasher().Generate(userDTO.Password);
        }
        
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task DeleteUser(string username)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);
        
        if (user is null)
            throw new EntityNotFoundException($"User with username={username} was not found");
        
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> Register(UserDTO userDTO)
    {
        if (IsUserRegistered(userDTO.UserName!))
            throw new NotUniqueUserNameException($"User with username={userDTO.UserName} already registered");

        if (new PasswordValidator(userDTO.Password!).Validate() is false)
            throw new InvalidPasswordException(
                "Password must be at least 8 symbols and contain one digit and one special character!");
        
        User user = new()
        {
            UserName = userDTO.UserName!,
            Password = new PasswordHasher().Generate(userDTO.Password!),
            Email = userDTO.Email!
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<JWTPair> Login(UserLoginRequestDTO data)
    {
        return await _jwtProvider.Generate(data.UserName);
    }

    public async Task<JWTPair> Refresh(string username)
    {
        return await _jwtProvider.Generate(username);
    }

    public bool IsUserRegistered(string username)
    {
        return _dbContext.Users.AsParallel().FirstOrDefault(u => u.UserName == username) is not null;
    }
}
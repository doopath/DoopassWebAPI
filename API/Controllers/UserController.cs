using Doopass.API.Domain.DTOs;
using Doopass.API.Infrastructure;
using Doopass.API.Infrastructure.Services;
using Doopass.API.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Doopass.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    protected readonly IUserService _userService;

    public UserController(DoopassContext dbContext, IConfiguration configuration)
    {
        JWTConfig jwtConfig = new(
            issuer: configuration["JwtSettings:Issuer"]!,
            audience: configuration["Audience"]!,
            lifeTime: double.Parse(configuration["JwtSettings:LifeTime"]!),
            secretKeyVariableName: configuration["JwtSettings:KeyEnvVariable"]!
        );
        _userService = new UserService(dbContext, jwtConfig);
    }

    [HttpPost]
    [Route("auth/register")]
    public async Task<ActionResult<User>> Create(UserDTO userDTO)
    {
        User user = await _userService.Register(userDTO);
        return Ok(user);
    }

    [HttpPost]
    [Route("auth/login")]
    public async Task<ActionResult<JWTTokenDTO>> Login(UserLoginRequestDTO dto)
    {
        User? user = await _userService.GetUser(dto.UserName);

        if (user is null)
            return BadRequest($"User with username={dto.UserName} was not found");

        if (!new PasswordHasher().Verify(dto.Password, user.Password))
            return BadRequest("Invalid password");

        string token = await _userService.Login(dto);
        JWTTokenDTO tokenDTO = new() {
            AccessToken = token,
            UserName = dto.UserName
        };

        return Ok(tokenDTO);
    }
}
using Doopass.API.Domain.DTOs;
using Doopass.API.Infrastructure;
using Doopass.API.Infrastructure.Services;
using Doopass.API.Domain.Models;
using Doopass.API.Domain;
using Microsoft.AspNetCore.Mvc;
using Doopass.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Doopass.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(DoopassContext dbContext, IConfiguration configuration)
    {
        JWTConfig jwtConfig = new(
            configuration["JwtSettings:Issuer"]!,
            configuration["Audience"]!,
            double.Parse(configuration["JwtSettings:AccessLifetime"]!),
            double.Parse(configuration["JwtSettings:RefreshLifetime"]!),
            configuration["JwtSettings:KeyEnvVariable"]!
        );
        _userService = new UserService(dbContext, jwtConfig);
    }

    [HttpPost]
    [Route("auth/register")]
    public async Task<ActionResult<UserDTO>> Create(UserDTO userDTO)
    {
        try
        {
            var user = await _userService.Register(userDTO);
            return Ok(user.ToDTO());
        }
        catch (InfrastructureBaseException exc)
        {
            return BadRequest(exc.Message);
        }
    }

    [HttpPost]
    [Route("auth/login")]
    public async Task<ActionResult<JWTTokenDTO>> Login(UserLoginRequestDTO dto)
    {
        var user = await _userService.GetUser(dto.UserName);

        if (user is null)
            return BadRequest($"User with username={dto.UserName} was not found");

        if (!new PasswordHasher().Verify(dto.Password, user.Password))
            return BadRequest("Invalid password");

        var pair = await _userService.Login(dto);
        JWTTokenDTO tokenDTO = new()
        {
            AccessToken = pair.AccessToken,
            RefreshToken = pair.RefreshToken
        };

        return Ok(tokenDTO);
    }

    [HttpGet]
    [Authorize]
    [Route("auth/refresh")]
    public async Task<ActionResult<JWTTokenDTO>> Refresh()
    {
        var username = User.Identity!.Name;
        var pair = await _userService.Refresh(username!);
        JWTTokenDTO tokenDTO = new()
        {
            AccessToken = pair.AccessToken,
            RefreshToken = pair.RefreshToken
        };

        return Ok(tokenDTO);
    }

    [HttpGet]
    [Route("list")]
    public async Task<ActionResult<List<UserDTO>>> GetUsers()
    {
        var users = await _userService.GetUsers();
        var DTOs = users.AsParallel().Select(u => u.ToDTO()).ToList();
        
        return Ok(DTOs);
    }

    [Authorize]
    [HttpGet]
    [Route("{username}")]
    public async Task<ActionResult<UserDTO>> GetUser(string username)
    {
        var user = await _userService.GetUser(username);

        if (user is null)
            return BadRequest($"User with username={username} was not found");

        return Ok(user.ToDTO());
    }

    [Authorize]
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<UserDTO>> UpdateUser(UserDTO userDTO)
    {
        try
        {
            var user = await _userService.UpdateUser(userDTO);
            return Ok(user.ToDTO());
        }
        catch (EntityNotFoundException exc)
        {
            return BadRequest(exc.Message);
        }
    }

    [Authorize]
    [HttpDelete]
    [Route("delete/{username}")]
    public async Task<ActionResult> DeleteUser(string username)
    {
        try
        {
            await _userService.DeleteUser(username);
        }
        catch (EntityNotFoundException exc)
        {
            return BadRequest(exc.Message);
        }

        return Ok();
    }
}
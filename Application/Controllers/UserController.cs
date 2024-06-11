using System.Text.Json;
using Domain;
using Domain.DTOs;
using Infrastructure;
using Infrastructure.Contracts;
using Infrastructure.Exceptions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = NLog.ILogger;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UserController> _logger;

    public UserController(DoopassContext dbContext, IConfiguration configuration, IDistributedCache cache, ILogger<UserController> logger)
    {
        JWTConfig jwtConfig = new(
            configuration["JwtSettings:Issuer"]!,
            configuration["Audience"]!,
            double.Parse(configuration["JwtSettings:AccessLifetime"]!),
            double.Parse(configuration["JwtSettings:RefreshLifetime"]!),
            configuration["JwtSettings:KeyEnvVariable"]!
        );
        
        _userService = new UserService(dbContext, jwtConfig);
        _cacheService = new CacheService(cache);
        _logger = logger;
    }

    [HttpPost]
    [Route("auth/register")]
    public async Task<ActionResult<UserDTO>> Create(UserDTO userDTO)
    {
        try
        {
            var cacheKey = userDTO.ToCacheKey();
            var cached = _cacheService.Get(cacheKey);
            
            _logger.LogDebug("Checking if the user is already registered");

            if (await cached is not null)
            {
                var existing = _cacheService.Get(cacheKey);
                _logger.LogDebug($"User with username={userDTO.UserName} is already registered");
                _logger.LogDebug($"Returning cached data");
                return Ok(JsonSerializer.Deserialize<UserDTO>((await existing)!));
            }
            
            var register = _userService.Register(userDTO);
            
            _logger.LogDebug($"Registering new user with username={userDTO.UserName}");
            
            var user = await register;
            var dto = user.ToDTO();

            _logger.LogDebug("Adding new user to the cache");
            
            await _cacheService.Add(cacheKey, JsonSerializer.Serialize(dto));
            await _cacheService.Delete("usersList");
                
            return Ok(dto);
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
        var cached = await _cacheService.Get("usersList");
        
        if (cached is not null)
            return Ok(JsonSerializer.Deserialize<List<UserDTO>>(cached));
        
        var users = await _userService.GetUsers();
        
        var DTOs = users.AsParallel().Select(u => u.ToDTO()).ToList();
        
        await _cacheService.Add("usersList", JsonSerializer.Serialize(DTOs));
        
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
            var cacheKey = JsonSerializer.Serialize(userDTO);
            var cached = await _cacheService.Get(cacheKey);

            if (cached is not null)
                return Ok(JsonSerializer.Deserialize<UserDTO>(cached));

            var user = await _userService.UpdateUser(userDTO);
            var dto = user.ToDTO();
            
            await _cacheService.Add(cacheKey, JsonSerializer.Serialize(dto));
            
            return Ok(dto);
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
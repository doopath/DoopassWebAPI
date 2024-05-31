using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Doopass.API.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace Doopass.API.Infrastructure;

public class JWTProvider
{
    private readonly JWTConfig _config;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserService _userService;

    public JWTProvider(JWTConfig config, IPasswordHasher passwordHasher, IUserService userService)
    {
        _config = config;
        _passwordHasher = passwordHasher;
        _userService = userService;
    }

    public async Task<string> Generate(string username)
    {
        var identity = await GetIdentity(username);
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(_config.LifeTime)),
            signingCredentials: new(
                GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            )
        );
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }

    private async Task<ClaimsIdentity> GetIdentity(string username)
    {
        List<User> users = await _userService.GetUsers();
        User user = users.AsParallel().First(u => u.UserName == username);
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Email),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Password)
        };

        ClaimsIdentity claimsIdentity = new(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        return claimsIdentity;
    }

    private SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new(Encoding.UTF8.GetBytes(_config.SecretKey));
    }
}
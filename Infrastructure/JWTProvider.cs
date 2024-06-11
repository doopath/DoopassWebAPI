using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Models;
using Infrastructure.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public class JWTProvider(JWTConfig config, IPasswordHasher passwordHasher, IUserService userService)
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<JWTPair> Generate(string username)
    {
        var now = DateTime.UtcNow;
        var identity = await GetIdentity(username);

        var accessExpires = now.AddMinutes(config.AccessLifetime);
        var refreshExpires = now.AddMinutes(config.RefreshLifetime);

        var accessToken = GenerateToken(identity, accessExpires);
        var refreshToken = GenerateToken(identity, refreshExpires);

        var encodeAccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);
        var encodeRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);

        return new JWTPair
        {
            AccessToken = encodeAccessToken,
            RefreshToken = encodeRefreshToken
        };
    }

    private async Task<ClaimsIdentity> GetIdentity(string username)
    {
        List<User> users = await userService.GetUsers();
        var user = users.AsParallel().First(u => u.UserName == username);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey));
    }

    private JwtSecurityToken GenerateToken(ClaimsIdentity identity, DateTime expires)
    {
        var now = DateTime.UtcNow;

        return new JwtSecurityToken(
            config.Issuer,
            config.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            )
        );
    }
}
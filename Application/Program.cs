using System.Text;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var dbOptions = new DbOptions();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidAudience = config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable(
                        config["JwtSettings:KeyEnvVariable"]!
                    )!
                )
            ),
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<DoopassContext>(options => options.UseNpgsql(dbOptions.ConnectionString));
builder.Services.AddAuthorization();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = new DistributedCacheOptions().ConnectionString;
    options.InstanceName = "doopass";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception exc)
{
    NLog.LogManager.GetCurrentClassLogger().Error(exc);
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
using Doopass.API.DTOs;
using Doopass.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Doopass.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(DoopassContext dbContext) : ControllerBase
{
    protected readonly DoopassContext _dbContext = dbContext;

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<User>> Create(UserDTO userDTO)
    {
        User user = new() {
            UserName = userDTO.UserName!,
            Password =  userDTO.Password!,
            Email = userDTO.Email!,
        };

        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return new ActionResult<User>(user);
    }
}
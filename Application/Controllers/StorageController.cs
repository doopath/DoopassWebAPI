using System.Security.Claims;
using Domain;
using Domain.DTOs;
using Infrastructure.Contracts;
using Infrastructure.Exceptions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StorageController(DoopassContext dbContext) : ControllerBase
{
    private readonly IStorageService _storageService = new StorageService(dbContext);

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<StorageDTO>> GetStorage(int id)
    {
            var storage = await _storageService.GetStorage(id);
            
            if (storage is null)
                return BadRequest($"Storage with id={id} was not found!");
                
            return Ok(storage.ToDTO());
    }
    
    [HttpGet]
    [Route("of-user/{username}")]
    public async Task<ActionResult<List<StorageDTO>>> GetStoragesOfUser(string username)
    {
            var storages = await _storageService.GetStoragesOfUser(username);
            var DTOs = storages.ConvertAll(s => s.ToDTO());
                
            return Ok(DTOs);
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<StorageDTO>> CreateStorage(StorageDTO storageDTO)
    {
        try
        {
            storageDTO.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var storage = await _storageService.CreateStorage(storageDTO);

            return Ok(storage.ToDTO());
        }
        catch (ArgumentNullException)
        {
            return BadRequest("Required fields are missing!");
        }
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<StorageDTO>> UpdateStorage(StorageDTO storageDTO)
    {
        try
        {
            var storage = await _storageService.UpdateStorage(storageDTO);

            return Ok(storage.ToDTO());
        }
        catch (EntityNotFoundException exc)
        {
            return BadRequest(exc.Message);
        }
    }

    [HttpDelete]
    [Route("delete/{id:int}")]
    public async Task<ActionResult> DeleteStorage(int id)
    {
        try
        {
            await _storageService.DeleteStorage(id);

            return Ok();
        }
        catch (EntityNotFoundException exc)
        {
            return BadRequest(exc.Message);
        }
    }
}
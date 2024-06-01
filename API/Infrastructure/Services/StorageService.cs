using Doopass.API.Domain;
using Doopass.API.Domain.DTOs;
using Doopass.API.Domain.Models;
using Doopass.API.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Doopass.API.Infrastructure.Services;

public class StorageService(DoopassContext dbContext) : IStorageService
{
    public async Task<Storage?> GetStorage(int id)
    {
        return await dbContext.Storages.FirstOrDefaultAsync(storage => storage.Id == id);
    }

    public async Task<List<Storage>> GetStoragesOfUser(string username)
    {
        return await dbContext.Storages.Where(storage => storage.User.UserName == username).ToListAsync();
    }

    public async Task<Storage> CreateStorage(StorageDTO storageDTO)
    {
        Storage storage = new()
        {
            Name = storageDTO.Name!,
            User = await dbContext.Users.FirstAsync(user => user.Id == storageDTO.UserId),
            Content = storageDTO.Content!
        };

        await dbContext.Storages.AddAsync(storage);
        await dbContext.SaveChangesAsync();

        return storage;
    }

    public async Task<Storage> UpdateStorage(StorageDTO storageDTO)
    {
        var storage = await dbContext.Storages.FirstOrDefaultAsync(storage => storage.Id == storageDTO.Id);
        
        if (storage is null)
            throw new EntityNotFoundException($"Storage with id={storageDTO.Id} was not found!");
        
        storage.Name = storageDTO.Name ?? storage.Name;
        storage.Content = storageDTO.Content ?? storage.Content;
        
        await dbContext.SaveChangesAsync();
        
        return storage;
    }

    public async Task DeleteStorage(int id)
    {
        var storage = await dbContext.Storages.FirstOrDefaultAsync(storage => storage.Id == id);
        
        if (storage is null)
            throw new EntityNotFoundException($"Storage with id={id} was not found!");
        
        dbContext.Storages.Remove(storage);
        await dbContext.SaveChangesAsync();
    }
}
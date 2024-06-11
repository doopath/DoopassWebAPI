using Domain.DTOs;
using Domain.Models;

namespace Infrastructure.Contracts;

public interface IStorageService
{
    public Task<Storage?> GetStorage(int id);
    public Task<List<Storage>> GetStoragesOfUser(string username);
    public Task<Storage> CreateStorage(StorageDTO storageDTO);
    public Task<Storage> UpdateStorage(StorageDTO storageDTO);
    public Task DeleteStorage(int id);
}
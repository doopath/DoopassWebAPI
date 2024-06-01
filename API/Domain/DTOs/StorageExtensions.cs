using Doopass.API.Domain.Models;

namespace Doopass.API.Domain.DTOs;

public static class StorageExtensions
{
    public static StorageDTO ToDTO(this Storage storage)
    {
        return new()
        {
            Id = storage.Id,
            Name = storage.Name,
            Content = storage.Content,
            UserId = storage.UserId,
        };
    }
}
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain;

public class DoopassContext(DbContextOptions<DoopassContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; init; }
    public required DbSet<Storage> Storages { get; init; }
    public required DbSet<Backup> Backups { get; init; }
}
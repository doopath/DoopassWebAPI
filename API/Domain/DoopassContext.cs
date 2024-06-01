using Microsoft.EntityFrameworkCore;
using Doopass.API.Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Doopass.API.Domain;

public class DoopassContext(DbContextOptions<DoopassContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<Storage> Storages { get; init; }
    public DbSet<Backup> Backups { get; init; }
}
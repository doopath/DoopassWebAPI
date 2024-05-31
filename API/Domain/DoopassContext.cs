using Microsoft.EntityFrameworkCore;
using Doopass.API.Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Doopass.API.Domain;

public class DoopassContext : DbContext
{
    public DoopassContext(DbContextOptions<DoopassContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Backup> Backups { get; set; }
}
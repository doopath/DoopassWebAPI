using Microsoft.EntityFrameworkCore;

namespace Doopass.API.Domain.Models;

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
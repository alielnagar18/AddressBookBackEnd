using Microsoft.EntityFrameworkCore;
using AddressBook.Core.Entities;
using BCrypt.Net;

namespace AddressBook.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Job>().HasData(new Job { Id = 1, Name = "Developer" }, new Job { Id = 2, Name = "Manager" });
        builder.Entity<Department>().HasData(new Department { Id = 1, Name = "IT" }, new Department { Id = 2, Name = "HR" });

    }
}

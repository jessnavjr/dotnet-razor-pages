using DotNetRazorPages.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetRazorPages.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.JobTitle).HasMaxLength(150).IsRequired();
            entity.Property(e => e.HireDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
        });
    }
}
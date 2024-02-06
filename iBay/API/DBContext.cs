using API.Models;
namespace API;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Cart>()
            .HasOne<User>(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Cart>()
            .HasOne<Product>(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId);

        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Product>()
            .HasOne<User>(p => p.User)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.SellerId);

        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasMany<Cart>(u => u.Carts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<User>()
            .HasMany<Product>(u => u.Products)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.SellerId);
    }*/
}
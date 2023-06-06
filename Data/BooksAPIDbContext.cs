using BooksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Data;

public class BooksApiDbContext : DbContext
{
    public BooksApiDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Book> Books { get; set; } = null!;
    
    public DbSet<Author> Authors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BooksApiDbContext).Assembly);
    }
}
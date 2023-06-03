using BooksAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Data;

public class BooksApiDbContext : DbContext
{
    public BooksApiDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Book> Books { get; set; } = null!;
}
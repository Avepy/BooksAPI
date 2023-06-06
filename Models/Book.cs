using System.ComponentModel.DataAnnotations.Schema;

namespace BooksAPI.Models;

public sealed class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Author AuthorNavigation { get; set; } = null!;
    public Guid AuthorId { get; set; }
}
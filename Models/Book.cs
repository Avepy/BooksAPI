namespace BooksAPI.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public virtual Author AuthorNavigation { get; set; } = null!;
    public Guid AuthorId { get; set; }
}
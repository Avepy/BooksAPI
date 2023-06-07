using Newtonsoft.Json;

namespace BooksAPI.Models;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public ICollection<Book> Books { get; set; } = null!;
}
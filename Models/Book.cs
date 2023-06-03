using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models;

public class Book
{
    [Required] public Guid Id { get; set; }
    [Required] public string Title { get; set; } = null!;
    
    [Required] public string Description { get; set; } = null!;

    [Required] public string Author { get; set; } = null!;
}
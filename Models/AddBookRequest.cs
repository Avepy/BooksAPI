using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models;

public class AddBookRequest
{
    [Required] public string Title { get; set; } = null!;
    [Required] [MaxLength(500)] public string Description { get; set; } = null!;
    [Required] public string Author { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models;

public class UpdateBookRequest
{
    public Guid Id { get; init; }
    [Required] public string Title { get; init; } = null!;
    [MaxLength(500)] public string Description { get; init; } = null!;
    [Required] public string Author { get; init; } = null!;
}
using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models;

public class AddAuthorRequest
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string Surname { get; set; } = null!;
}
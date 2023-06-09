using BooksAPI.Data;
using BooksAPI.Identity;
using BooksAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BooksController : Controller
{
    private readonly BooksApiDbContext _dbContext;

    public BooksController(BooksApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        return Ok(await _dbContext.Books.AsNoTracking().Include(b => b.AuthorNavigation).ToListAsync());
    }
    
    [HttpGet ("authors")]
    public async Task<IActionResult> GetAuthors()
    {
        return Ok(await _dbContext.Authors.AsNoTracking().Include(a => a.Books).ToListAsync());
    }

    [HttpGet ("{id:guid}")]
    public async Task<IActionResult> GetBook([FromRoute] Guid id)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }
    
    [HttpGet ("authors/{id:guid}")]
    public async Task<IActionResult> GetAuthor([FromRoute] Guid id)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        return Ok(author);
    }
    
    [Authorize]
    [HttpPost ("authors")]
    public async Task<IActionResult> AddAuthor(AddAuthorRequest addAuthorRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var authorExists = await _dbContext.Authors.AnyAsync(a => a.Name == addAuthorRequest.Name && a.Surname == addAuthorRequest.Surname);
        
        if (authorExists)
        {
            return Conflict("Author already exists");
        }

        var author = new Author()
        {
            Id = Guid.NewGuid(),
            Name = addAuthorRequest.Name,
            Surname = addAuthorRequest.Surname
        };
        
        await _dbContext.Authors.AddAsync(author);
        await _dbContext.SaveChangesAsync();

        return Created("", author);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddBook(AddBookRequest addBookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var separatedAuthor = addBookRequest.Author.Split(" ");
        
        if (separatedAuthor.Length < 2)
        {
            return BadRequest("Please provide both the author's name and surname");
        }
        
        var authorName = separatedAuthor[0];
        var authorSurname = string.Join(" ", separatedAuthor.Skip(1));

        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName && a.Surname == authorSurname);
        
        if (author == null)
        {
            return BadRequest("The specified author does not exist");
        }

        var book = new Book()
        {
            Id = Guid.NewGuid(),
            Title = addBookRequest.Title,
            Description = addBookRequest.Description,
            AuthorId = author.Id
        };

        await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync();

        return Created("", book);
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateBook([FromBody] UpdateBookRequest updateBookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == updateBookRequest.Id);

        if (book == null) return NotFound();
        
        var authorName = updateBookRequest.Author.Split(" ")[0];
        var authorSurname = updateBookRequest.Author.Split(" ")[1];
        
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName && a.Surname == authorSurname);
        
        if (author == null)
        {
            return BadRequest("Author does not exist");
        }

        book.Title = updateBookRequest.Title;
        book.Description = updateBookRequest.Description;
        book.AuthorNavigation = author;

        await _dbContext.SaveChangesAsync();
        
        return Ok(book);
    }

    [Authorize]
    [RequiresClaim(IdentityData.AdminUserClaimName, "true")]
    [HttpDelete ("{id:guid}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();
        
        _dbContext.Remove(book);
        await _dbContext.SaveChangesAsync();
        
        return Ok(book);
    }
    
    [Authorize]
    [RequiresClaim(IdentityData.AdminUserClaimName, "true")]
    [HttpDelete ("authors/{id:guid}")]
    public async Task<IActionResult> DeleteAuthor([FromRoute] Guid id)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

        if (author == null) return NotFound();
        
        _dbContext.Remove(author);
        await _dbContext.SaveChangesAsync();
        
        return Ok(author);
    }
}
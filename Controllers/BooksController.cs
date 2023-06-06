using BooksAPI.Data;
using BooksAPI.Models;
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
        return Ok(await _dbContext.Books.AsNoTracking().ToListAsync());
    }
    
    [HttpGet ("Authors")]
    public async Task<IActionResult> GetAuthors()
    {
        return Ok(await _dbContext.Authors.AsNoTracking().ToListAsync());
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
    
    [HttpGet ("Authors/{id:guid}")]
    public async Task<IActionResult> GetAuthor([FromRoute] Guid id)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        return Ok(author);
    }
    
    [HttpPost ("Authors")]
    public async Task<IActionResult> AddAuthor(AddAuthorRequest addAuthorRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var authorExists = await _dbContext.Authors.AnyAsync(a => a.Name == addAuthorRequest.Name && a.Surname == addAuthorRequest.Surname);
        
        if (authorExists)
        {
            return Conflict();
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

    [HttpPost]
    public async Task<IActionResult> AddBook(AddBookRequest addBookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        if (addBookRequest.Author.Split(" ").Length != 2)
        {
            return BadRequest("Please provide the author's name and surname");
        }
        
        var authorName = addBookRequest.Author.Split(" ")[0];
        var authorSurname = addBookRequest.Author.Split(" ")[1];
        
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName && a.Surname == authorSurname);
        
        if (author == null)
        {
            return BadRequest("Please add the author first");
        }

        var book = new Book()
        {
            Id = Guid.NewGuid(),
            Title = addBookRequest.Title,
            Description = addBookRequest.Description,
            AuthorNavigation = author,
        };

        await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync();

        return Created("", book);
    }

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

    [HttpDelete ("{id:guid}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();
        
        _dbContext.Remove(book);
        await _dbContext.SaveChangesAsync();
        
        return Ok(book);
    }
    
    [HttpDelete ("Authors/{id:guid}")]
    public async Task<IActionResult> DeleteAuthor([FromRoute] Guid id)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

        if (author == null) return NotFound();
        
        _dbContext.Remove(author);
        await _dbContext.SaveChangesAsync();
        
        return Ok(author);
    }
}
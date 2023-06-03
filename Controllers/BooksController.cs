using BooksAPI.Data;
using BooksAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
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

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetBook([FromRoute] Guid id)
    {
        var book = await _dbContext.Books.SingleOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(AddBookRequest addBookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = new Book()
        {
            Id = Guid.NewGuid(),
            Title = addBookRequest.Title,
            Description = addBookRequest.Description,
            Author = addBookRequest.Author,
        };

        await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync();

        return Ok(book);
    }

    [HttpPatch] 
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateBook([FromRoute] Guid id, JsonPatchDocument<UpdateBookRequest> updateBookRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var book = await _dbContext.Books.SingleOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();

        var updateBook = new UpdateBookRequest
        {
            Title = book.Title,
            Description = book.Description,
            Author = book.Author
        };
        updateBookRequest.ApplyTo(updateBook);

        book.Title = updateBook.Title;
        book.Description = updateBook.Description;
        book.Author = updateBook.Author;

        await _dbContext.SaveChangesAsync();
        
        return Ok(book);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
    {
        var book = await _dbContext.Books.SingleOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();
        
        _dbContext.Remove(book);
        await _dbContext.SaveChangesAsync();
        return Ok(book);
    }
}
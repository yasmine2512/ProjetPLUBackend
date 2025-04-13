using Microsoft.AspNetCore.Mvc;
using MyBlazorApp.Data;
using MyBlazorApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MyBlazorBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

     [HttpGet]
         public async Task<ActionResult<IEnumerable<User>>> GetUsers()
         {
        return  await _context.Users  .ToListAsync();
    
         }


        [HttpPost]
 public async Task<IActionResult> CreateUser([FromBody] User user)
 {
     Console.WriteLine("0");
    var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
    Console.WriteLine($"Request Body: {requestBody}");

     if (!ModelState.IsValid)
     {
         Console.WriteLine("ModelState is not valid!");
        foreach (var entry in ModelState)
        {
            Console.WriteLine($"Key: {entry.Key}, Error: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
        }

      //   Console.WriteLine(JsonSerializer.Serialize(user));
         return BadRequest(ModelState);
     }

     Console.WriteLine($"Received: {JsonSerializer.Serialize(user)}");

     _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(user);
 }

 [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel login)
{
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);

    if (user == null)
        return Unauthorized(new { message = "Invalid credentials" });

    return Ok(user);
}
}

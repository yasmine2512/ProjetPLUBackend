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


 [HttpPost("upload-profile-picture/{userId}")]
 public async Task<IActionResult> UploadProfilePicture(int userId, IFormFile file)
 {
     if (file == null || file.Length == 0)
         return BadRequest("No file uploaded.");

     var user = await _context.Users.FindAsync(userId);
     if (user == null)
         return NotFound();

     var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pictures");
     if (!Directory.Exists(uploadsFolder))
         Directory.CreateDirectory(uploadsFolder);

     var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
     var filePath = Path.Combine(uploadsFolder, fileName);

     using (var stream = new FileStream(filePath, FileMode.Create))
     {
         await file.CopyToAsync(stream);
     }

     user.PicturePath = $"profile_pictures/{fileName}";
     await _context.SaveChangesAsync();

     return Ok(new { path = user.PicturePath });
 }

[HttpGet("{id}")]
public ActionResult<User> GetUserById(int id)
{
    try
    {
        var user = _context.GetUserByID(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}



}

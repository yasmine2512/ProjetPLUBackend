using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorBackend.Models;
using MyBlazorApp.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetPLU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/favorite/user/3
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFavoritesByUser(int userId)
        {
            try
    {
        var favorites = await _context.Favorites
            .Where(f => f.UserID == userId)
            .Include(f => f.Memoire)
            .ThenInclude(m => m.Professor)
            .Include(f => f.User) 
            .Select(f => new
            {
                f.UserID,
                f.MemoireID,
              
                Memoire = new
                {
                    f.Memoire.MemoireID,
                    f.Memoire.ProfessorID,
                    f.Memoire.Title,
                    f.Memoire.AuthorName,
                    f.Memoire.Field,
                    f.Memoire.Keywords,
                    f.Memoire.Date,
                    f.Memoire.FilePath,
                    ProfessorName = f.Memoire.Professor.FullName,
                    ProfessorPicturePath = f.Memoire.Professor.PicturePath
                  
                }
            })
            .ToListAsync();

        return Ok(favorites);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Erreur serveur: {ex.Message}");
    }
        }

        // POST: api/favorite
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteDto dto)
        {
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserID == dto.UserID && f.MemoireID == dto.MemoireID);

            if (exists)
                return BadRequest("Cette thèse est déjà dans les favoris.");
            var favorite = new Favorite
             {
               UserID = dto.UserID,
               MemoireID = dto.MemoireID
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok("Favori ajouté !");
        }

        // DELETE: api/favorite?userId=3&memoireId=7
        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite([FromQuery] int userId, [FromQuery] int memoireId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.MemoireID == memoireId);

            if (favorite == null)
                return NotFound("Favori non trouvé.");

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok("Favori supprimé !");
        }
    }
}

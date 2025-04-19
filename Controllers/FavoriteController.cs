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
            var favorites = await _context.Favorites
                .Where(f => f.UserID == userId)
                .Include(f => f.Memoire)
                .ToListAsync();

            return Ok(favorites);
        }

        // POST: api/favorite
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] Favorite favorite)
        {
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserID == favorite.UserID && f.MemoireID == favorite.MemoireID);

            if (exists)
                return BadRequest("Cette thèse est déjà dans les favoris.");

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

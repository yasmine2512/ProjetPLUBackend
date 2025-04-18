using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorBackend.Models;
using MyBlazorApp.Data;

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
				.AnyAsync(f => f.UserID == favorite.UserID && f.Memoire

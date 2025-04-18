using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorBackend.Models;
using MyBlazorApp.Data;

namespace ProjetPLU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // Obtenir tous les commentaires d'une thèse
        [HttpGet("memoire/{memoireId}")]
        public IActionResult GetByMemoire(int memoireId)
        {
            var comments = _context.Comments
                .Include(c => c.User)
                .Where(c => c.MemoireID == memoireId)
                .OrderByDescending(c => c.Date)
                .ToList();

            return Ok(comments);
        }

        // Ajouter un commentaire
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Text))
                return BadRequest("Le commentaire est vide");

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("Commentaire ajouté !");
        }

        //afficher la date devant le commentaire
        [HttpPost]
        [Authorize] // voir plus bas pour activer l'auth
        public async Task<IActionResult> Add([FromBody] Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Text))
                return BadRequest("Le commentaire est vide");

            comment.Date = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("Commentaire ajouté !");
        }

    }
}

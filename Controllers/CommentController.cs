using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorBackend.Models;
using MyBlazorApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: api/comment/memoire/5
        [HttpGet("memoire/{memoireId}")]
        public IActionResult GetByMemoire(int memoireId)
        {
            var comments = _context.Comments
        .Include(c => c.User)
        .Where(c => c.MemoireID == memoireId)
        .OrderByDescending(c => c.Date)
        .Select(c => new CommentDto
        {
            CommentID = c.CommentID,
            Text = c.Text,
            Date = c.Date,
            UserID = c.UserID,
            FullName = c.User.FullName,
            PicturePath = c.User.PicturePath
        })
        .ToList();

    return Ok(comments);
        }


 // POST: api/comment
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Text))
                return BadRequest("Le commentaire est vide");

            comment.Date = DateTime.Now;

 var user = await _context.Users.FindAsync(comment.UserID);
    if (user == null)
    {
        return BadRequest("Utilisateur non trouvé.");
    }

    // Optional: attach user to prevent EF from trying to create a new one
    comment.User = user;

    var memoire = await _context.Memoires.FindAsync(comment.MemoireID);
    if (memoire == null)
        return BadRequest("Mémoire non trouvé");

    comment.Memoire = memoire;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("Commentaire ajouté !");
        }

        // DELETE: api/comment/12
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
                return NotFound("Commentaire non trouvé");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Commentaire supprimé avec succès !");
        }
    }
}

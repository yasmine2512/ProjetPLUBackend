using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;

namespace MyBlazorApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemoiresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemoiresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/memoires/3
        [HttpGet("{id}")]
        public async Task<ActionResult<Memoire>> GetMemoire(int id)
        {
            var memoire = await _context.Memoires.FindAsync(id);

            if (memoire == null)
            {
                return NotFound();
            }

            return Ok(memoire);
        }
    }
}


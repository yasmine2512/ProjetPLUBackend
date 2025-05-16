using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;
using System.Data.Common;

namespace MyBlazorApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

      
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required.");
            }

            term = term.Trim().ToLowerInvariant();

            var suggestions = await _context.Memoires
                .Where(m =>
                    m.Title.ToLower().Contains(term) ||
                    m.AuthorName.ToLower().Contains(term) ||
                    m.Keywords.ToLower().Contains(term) ||
                    m.Field.ToLower().Contains(term)) 
                .Select(m => new {
                 m.MemoireID,
                 m.Title,
                 m.AuthorName,
                 m.FilePath,
                 m.ProfessorID,
                 ProfessorName = m.Professor.FullName,
                 ProfessorPicturePath = m.Professor.PicturePath,
                 m.Date,
                 m.Field,
                 m.Keywords
                  })
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Ok(suggestions);
        }

       
        [HttpGet("similar/{memoireId}")]
        public async Task<IActionResult> GetSimilarMemoires(int memoireId)
        {
            var results = new List<Memoire>();
            var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var command = conn.CreateCommand();
            command.CommandText = "CALL GetSimilarMemoires(@memoireId)";
            var param = command.CreateParameter();
            param.ParameterName = "@memoireId";
            param.Value = memoireId;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new Memoire
                {
                    MemoireID = reader.GetInt32(reader.GetOrdinal("MemoireID")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Keywords = reader.IsDBNull(reader.GetOrdinal("Keywords")) ? "" : reader.GetString(reader.GetOrdinal("Keywords")),
                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                    Field = reader.GetString(reader.GetOrdinal("Field")),
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    ProfessorID = reader.GetInt32(reader.GetOrdinal("ProfessorID"))
                });
            }

            await conn.CloseAsync();
            return Ok(results);
        }
    }
}

    

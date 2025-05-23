using Microsoft.AspNetCore.Mvc;
using MyBlazorApp.Data;
using System;
using System.Collections.Generic;
using MyBlazorBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace ProjetPLU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheseController : ControllerBase
    {
        private readonly AppDbContext _repository;
      

        public TheseController(AppDbContext context, ThesisSummarizerService summarizer)
        {
           _repository = context;
        
        }
        
   [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var theses = _repository.GetAllTheses();
                return Ok(theses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la récupération des thèses : " + ex.Message);
            }
        }
        // GET: api/these/user/{id}
        [HttpGet("user/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var these = _repository.GetById(id);
                if (these == null)
                    return NotFound($"Thèse avec l'ID {id} non trouvée.");

                return Ok(these);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la récupération de la thèse : " + ex.Message);
            }
        }

        // GET: api/these/{id}
        [HttpGet("{id}")]
        public IActionResult GetTheseById(int id)
        {
            try
            {
                var these = _repository.GetTheseById(id);
                if (these == null)
                    return NotFound($"Thèse avec l'ID {id} non trouvée.");

                return Ok(these);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la récupération de la thèse : " + ex.Message);
            }
        }

        // POST: api/these
        [HttpPost]
        public IActionResult Add([FromBody] Memoire these)
        {
            try
            {
                _repository.Add(these);
                return CreatedAtAction(nameof(GetById), new { id = these.MemoireID }, these);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de l'ajout de la thèse : " + ex.Message);
            }
        }

        // PUT: api/these/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Memoire these)
        {
            try
            {
                // Vérification si la thèse existe
                var existing = _repository.GetById(id);
                if (existing == null)
                    return NotFound($"Aucune thèse avec l'ID {id}");

                these.MemoireID = id; // Assurer que l'ID est bien pris
                _repository.Update(these);

                return Ok("Thèse mise à jour avec succès !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la mise à jour de la thèse : " + ex.Message);
            }
        }

        // DELETE: api/these/{id}
       [HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    try
    {
        // Find the thesis by ID
        var memoire = await _repository.Memoires
                            .FirstOrDefaultAsync(m => m.MemoireID == id);

        if (memoire == null)
            return NotFound($"Aucune thèse avec l'ID {id}");

        // Load and remove associated comments
        var comments = await _repository.Comments
                              .Where(c => c.MemoireID == id)
                              .ToListAsync();

        _repository.Comments.RemoveRange(comments);

        // Remove the thesis
        _repository.Memoires.Remove(memoire);

        // Save all changes
        await _repository.SaveChangesAsync();

        return Ok("Thèse et ses commentaires supprimés avec succès !");
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Erreur lors de la suppression : " + ex.Message);
    }
}
       

        [HttpPost("upload")]
public async Task<IActionResult> UploadThesis([FromForm] ThesisUploadDto dto, [FromForm] IFormFile file)
{
try{
     if (file == null || file.Length == 0)
        {return BadRequest("No file uploaded");
        Console.WriteLine("⚠️ No file selected.");
      }

    // Save the file
    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "theses");
    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
    var filePath = Path.Combine(uploadsFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // Save thesis record in DB
    var memoire = new Memoire
    {
        Title = dto.Title,
        Field = dto.Field,
        Keywords = dto.Keywords,
        AuthorName = dto.AuthorName,
        Date = DateTime.Now,
        ProfessorID = dto.ProfessorID, 
        FilePath = $"theses/{fileName}" 
    };

  
        _repository.Memoires.Add(memoire);
    await _repository.SaveChangesAsync();

    return Ok();}
     catch (Exception ex)
    {
        return BadRequest($"Upload failed: {ex.Message}");
    
    }

        
}
 public async Task<List<Memoire>> SearchThesesAsync(string query)
    {
        var results = await _repository.Memoires
            .FromSqlInterpolated($"CALL SearchThesesFullText({query})")
            .ToListAsync();

        
        if (!results.Any())
        {
            results = await _repository.Memoires
                .Where(m =>
                    EF.Functions.Like(m.Title, $"%{query}%") ||
                    EF.Functions.Like(m.AuthorName, $"%{query}%") ||
                    EF.Functions.Like(m.Keywords, $"%{query}%"))
                .ToListAsync();
        }

        return results;
    }

    }
}

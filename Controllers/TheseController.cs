Microsoft.AspNetCore.Mvc;
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

        public TheseController(AppDbContext context)
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

        // GET: api/theses/{id}
        [HttpGet("{id}")]
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

        // POST: api/theses
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

        // PUT: api/theses/{id}
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

        // DELETE: api/theses/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Vérifier si la thèse existe avant la suppression
                var existing = _repository.GetById(id);
                if (existing == null)
                    return NotFound($"Aucune thèse avec l'ID {id}");

                _repository.DeleteThesis(id);
                return Ok("Thèse supprimée avec succès !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la suppression de la thèse : " + ex.Message);
            }
        }

       
   
    

// [HttpGet]
// public async Task<ActionResult<IEnumerable<Memoire>>> GetAll()
// {
//     var memoires = await _context.Memoires.ToListAsync();
//     return Ok(memoires);
// }

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

        
}}
}

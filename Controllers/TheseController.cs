using Microsoft.AspNetCore.Mvc;
using ProjetPLU.Data;
using ProjetPLU.Models;
using System;
using System.Collections.Generic;

namespace ProjetPLU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheseController : ControllerBase
    {
        private readonly TheseRepository _repository;

        public TheseController(TheseRepository repository)
        {
            _repository = repository;
        }

        // GET: api/theses
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var theses = _repository.GetAll();
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
        public IActionResult Add([FromBody] These these)
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
        public IActionResult Update(int id, [FromBody] These these)
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

                _repository.Delete(id);
                return Ok("Thèse supprimée avec succès !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la suppression de la thèse : " + ex.Message);
            }
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using ProjetPLU.Data;
using ProjetPLU.Models;
using System;

namespace ProjetPLU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheseController : ControllerBase
    {
        private readonly TheseRepository _repository;

        public TheseController()
        {
            _repository = new TheseRepository();
        }

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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var these = _repository.GetById(id);
                if (these == null)
                    return NotFound($"Aucune thèse trouvée avec l'ID {id}");

                return Ok(these);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la récupération de la thèse : " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] These these)
        {
            try
            {
                _repository.Add(these);
                return Ok("Thèse ajoutée avec succès !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de l'ajout de la thèse : " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
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

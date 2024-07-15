using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Api_agricola.Models;

namespace Api_agricola.Controllers
{
    public class GruposController : Controller
    {
        private readonly AgricolaContext _context;

        public GruposController(AgricolaContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("Grupos/GetByLotId")]
        public async Task<IActionResult> GetByLotId(int? lotId)
        {
            if (lotId == null)
            {
                return NotFound(new { message = "Id is required" });
            }

            var grupos = await _context.Grupos
                .Where(g => g.IdLote == lotId)
                .ToListAsync();

            if (grupos == null || grupos.Count == 0)
            {
                return NotFound(new { message = "No groups found for the specified Lot id" });
            }

            return Ok(grupos);
        }

        [HttpGet]
        [Route("Grupos/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupo = await _context.Grupos
                .FirstOrDefaultAsync(g => g.Id == id);
            if (grupo == null)
            {
                return NotFound();
            }

            return Ok(grupo);
        }


        [HttpPost]
        [Route("Grupos/Create")]
        public async Task<IActionResult> Create([FromBody] Grupo grupo)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int lastId = await _context.Grupos.MaxAsync(g => (int?)g.Id) ?? 0;

            try
            {
                var newGrupo = new Grupo
                {
                    Id = lastId + 1,
                    IdLote= grupo.IdLote,
                    Nombre = grupo.Nombre,
                };

                _context.Add(newGrupo);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Lote creado exitosamente", id = newGrupo.Id });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al guardar el lote. Inténtalo de nuevo." + ex);
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error al guardar los cambios. Inténtalo de nuevo." + ex);
            }
        }

        [HttpPut]
        [Route("Grupos/Edit")]
        public async Task<IActionResult> Edit(int? id, [FromBody] Grupo grupo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != grupo.Id)
            {
                return NotFound();
            }

            var grupos = await _context.Grupos.FirstOrDefaultAsync(g => g.Id == id);

            if (grupo == null)
            {
                return NotFound();
            }

            grupos.Nombre = grupo.Nombre;

            await _context.SaveChangesAsync();
            return Ok();
        }




        [HttpDelete]
        [Route("Grupos/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var grupos = await _context.Grupos.FirstOrDefaultAsync(l => l.Id == id);

            if (grupos == null)
            {
                return NotFound(new { message = "grupo no encontrado" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Grupos.Remove(grupos);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Grupos deleted successfully" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}

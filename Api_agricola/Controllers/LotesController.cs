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
    public class LotesController : Controller
    {
        private readonly AgricolaContext _context;

        public LotesController(AgricolaContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Lotes/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(new { message = "Id is required" });
            }

            var lote = await _context.Lotes
                .FirstOrDefaultAsync(l => l.Id == id);
            if (lote == null)
            {
                return NotFound();
            }

            return Ok(lote);
        }

        [HttpGet]
        [Route("Lotes/GetByFarmId")]
        public async Task<IActionResult> GetByFarmId(int? farmId)
        {
            if (farmId == null)
            {
                return NotFound(new { message = "Id is required" });
            }

            var lotes = await _context.Lotes
                .Where(l => l.IdFinca == farmId)
                .ToListAsync();

            if (lotes == null || lotes.Count == 0)
            {
                return NotFound(new { message = "No lots found for the specified farm id" });
            }

            return Ok(lotes);
        }
        [HttpPost]
        [Route("Lotes/Create")]
        public async Task<IActionResult> Create([FromBody] Lote lote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int lastId = await _context.Lotes.MaxAsync(l => (int?)l.Id) ?? 0;

            try
            {
                var newLote = new Lote
                {
                    Id = lastId + 1,
                    IdFinca = lote.IdFinca,
                    Nombre = lote.Nombre,
                    Arboles = lote.Arboles,
                    Etapa = lote.Etapa,
                };

                _context.Add(newLote);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Lote creado exitosamente", id = newLote.Id });
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
        [Route("Lotes/Edit")]
        
        public async Task<IActionResult> Edit(int id, [FromBody] Lote lote)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lote.Id)
            {
                return NotFound();
            }

           var lotes = await _context.Lotes.FirstOrDefaultAsync(l => l.Id == id);

            if (lotes == null)
            {
                return NotFound();
            }

          
                lotes.Nombre = lote.Nombre;
                lotes.Arboles = lote.Arboles;
                lotes.Etapa = lote.Etapa;

            

           
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete]
        [Route("Lotes/Delete")]
        
        public async Task<IActionResult> Delete(int id)
        {
            var lotes = await _context.Lotes.FirstOrDefaultAsync(l => l.Id == id);

            if (lotes == null)
            {
                return NotFound(new { message = "lote no encontrado" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var grupos =await _context.Grupos.Where(g => g.IdLote == id).ToListAsync();

            while (grupos.Count > 0)
            {
                foreach (var grupo in grupos)
                {
                    _context.Grupos.Remove(grupo);
                }
                await _context.SaveChangesAsync();
                grupos = await _context.Grupos.Where(g => g.IdLote == id).ToListAsync();
            }


            _context.Lotes.Remove(lotes);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Finca and related Lotes deleted successfully" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

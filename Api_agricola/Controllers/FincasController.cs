using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Api_agricola.Models;
using Microsoft.AspNetCore.Http.Features;


namespace Api_agricola.Controllers
{
    public class FincasController : Controller
    {
        private readonly AgricolaContext _context;

        public FincasController(AgricolaContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Fincas/GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var fincas = await _context.Fincas.ToListAsync();
            return Ok(fincas);
        }


        // GET: Fincas/Details/5
        [HttpGet]
        [Route("Fincas/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(new { message = "Id is required" });
            }

            var finca = await _context.Fincas
                .FirstOrDefaultAsync(f => f.Id == id);
            if (finca == null)
            {
                return NotFound(new { message = "Finca not found" });
            }

            return Ok(finca);
        }

        [HttpPost]
        [Route("Fincas/Create")]
        public async Task<IActionResult> Create([FromBody] Finca finca)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int lastId = await _context.Fincas.MaxAsync(f => (int?)f.Id) ?? 0;

            try
            {
                var newFinca = new Finca
                {
                    Id = lastId + 1,
                    Nombre = finca.Nombre,
                    Ubicacion = finca.Ubicacion,
                    Hectareas = finca.Hectareas,
                    Descripcion = finca.Descripcion
                };

                _context.Add(newFinca);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Finca creada exitosamente", id = newFinca.Id });
            }
            catch (DbUpdateException ex)
            { 
                return StatusCode(500, "Error al guardar la finca. Inténtalo de nuevo." + ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado. Inténtalo de nuevo."+ ex);
            }
        }

        [HttpPut]
        [Route("Fincas/Edit")]
        public async Task<IActionResult> Edit(int id, [FromBody] Finca finca)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFinca = await _context.Fincas.FirstOrDefaultAsync(f => f.Id == id);

            if (existingFinca == null)
            {
                return NotFound();
            }

            existingFinca.Nombre = finca.Nombre;
            existingFinca.Ubicacion = finca.Ubicacion;
            existingFinca.Hectareas = finca.Hectareas;
            existingFinca.Descripcion = finca.Descripcion;

            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete]
        [Route("Fincas/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var finca = await _context.Fincas.FirstOrDefaultAsync(f => f.Id == id);
            if (finca == null)
            {
                return NotFound(new { message = "Finca not found" });
            }

            var lotes = await _context.Lotes.Where(l => l.IdFinca == id).ToListAsync();
            while (lotes.Count > 0)
            {
                foreach (var lote in lotes)
                {
                    _context.Lotes.Remove(lote);
                }
                await _context.SaveChangesAsync(); 
                lotes = await _context.Lotes.Where(l => l.IdFinca == id).ToListAsync();
            }
            _context.Fincas.Remove(finca);

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

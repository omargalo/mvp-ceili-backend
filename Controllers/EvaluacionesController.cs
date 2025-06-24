using CeiliApi.Data;
using CeiliApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CeiliApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluacionesController : ControllerBase
    {
        private readonly CeiliDbContext _db;

        public EvaluacionesController(CeiliDbContext db)
        {
            _db = db;
        }

        private int GetDocenteId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet]
        public async Task<IActionResult> GetMisEvaluaciones()
        {
            var docenteId = GetDocenteId();
            var evaluaciones = await _db.Evaluaciones
                .Include(e => e.Alumno)
                .Where(e => e.DocenteId == docenteId)
                .ToListAsync();
            return Ok(evaluaciones);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvaluacion([FromBody] Evaluacion evaluacion)
        {
            var docenteId = GetDocenteId();
            evaluacion.DocenteId = docenteId;
            evaluacion.Fecha = DateTime.Now;
            _db.Evaluaciones.Add(evaluacion);
            await _db.SaveChangesAsync();
            return Ok(evaluacion);
        }
    }
}

using CeiliApi.Data;
using CeiliApi.Models.DTOs;
using CeiliApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
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
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();

            var dtos = evaluaciones.Select(e => new EvaluacionResumenDto
            {
                Id = e.Id,
                AlumnoNombre = e.Alumno.NombreCompleto,
                Fecha = e.Fecha,
                ObservacionDocente = e.ObservacionDocente,
                Aspectos = string.IsNullOrEmpty(e.AspectosJson)
            ? new List<AspectoEvaluadoDto>()
            : JsonSerializer.Deserialize<List<AspectoEvaluadoDto>>(e.AspectosJson) ?? new List<AspectoEvaluadoDto>()
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvaluacion([FromBody] EvaluacionDto dto)
        {
            var docenteId = GetDocenteId();

            // Mapeo DTO → Entidad
            var evaluacion = new Evaluacion
            {
                AlumnoId = dto.AlumnoId,
                DocenteId = docenteId,
                Fecha = DateTime.Now,
                AspectosJson = JsonSerializer.Serialize(dto.Aspectos),
                ObservacionDocente = dto.ObservacionDocente
            };

            _db.Evaluaciones.Add(evaluacion);
            await _db.SaveChangesAsync();
            return Ok(new { id = evaluacion.Id });
        }
    }
}

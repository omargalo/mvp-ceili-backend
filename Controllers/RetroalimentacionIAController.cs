using CeiliApi.Data;
using CeiliApi.Models.DTOs;
using CeiliApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CeiliApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RetroalimentacionIAController : ControllerBase
    {
        private readonly ApiDbContext _db;
        private readonly ChatGptService _chatService;

        public RetroalimentacionIAController(ApiDbContext db, ChatGptService chatService)
        {
            _db = db;
            _chatService = chatService;
        }

        // GET api/retroalimentacionia/evaluacion/5
        [HttpGet("evaluacion/{evaluacionId}")]
        public async Task<IActionResult> GetPorEvaluacion(int evaluacionId)
        {
            var retro = await _db.Retroalimentaciones
                .FirstOrDefaultAsync(r => r.EvaluacionId == evaluacionId);

            if (retro == null) return NotFound();

            var dto = new RetroalimentacionIADto
            {
                Id = retro.Id,
                EvaluacionId = retro.EvaluacionId,
                Texto = retro.Texto,
                FechaGeneracion = retro.FechaGeneracion,
                ModeloIA = retro.ModeloIA
            };

            return Ok(dto);
        }

        // POST api/retroalimentacionia
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] RetroalimentacionIADto dto)
        {
            // (Opcional) Verifica si ya existe para la evaluación
            var existente = await _db.Retroalimentaciones
                .FirstOrDefaultAsync(r => r.EvaluacionId == dto.EvaluacionId);

            if (existente != null)
                return Conflict("Ya existe retroalimentación para esta evaluación.");

            var retro = new RetroalimentacionIA
            {
                EvaluacionId = dto.EvaluacionId,
                Texto = dto.Texto,
                FechaGeneracion = dto.FechaGeneracion,
                ModeloIA = dto.ModeloIA
            };

            _db.Retroalimentaciones.Add(retro);
            await _db.SaveChangesAsync();

            dto.Id = retro.Id;
            return Ok(dto);
        }

        [HttpPost("generar/{evaluacionId}")]
        public async Task<IActionResult> GenerarRetroalimentacionIA(int evaluacionId)
        {
            var evaluacion = await _db.Evaluaciones
                .Include(e => e.Alumno)
                .Include(e => e.Docente)
                .FirstOrDefaultAsync(e => e.Id == evaluacionId);

            if (evaluacion == null) return NotFound();

            // (1) Checa si ya existe
            var existente = await _db.Retroalimentaciones.FirstOrDefaultAsync(r => r.EvaluacionId == evaluacionId);
            if (existente != null)
                return Conflict("Ya existe retroalimentación IA para esta evaluación.");

            // Lógica va al servicio
            var gptRespuesta = await _chatService.GenerarRetroalimentacionIAAsync(evaluacion);

            var retro = new RetroalimentacionIA
            {
                EvaluacionId = evaluacionId,
                Texto = gptRespuesta,
                FechaGeneracion = DateTime.UtcNow,
                ModeloIA = "gpt-4o-mini"
            };

            _db.Retroalimentaciones.Add(retro);
            await _db.SaveChangesAsync();

            var dto = new RetroalimentacionIADto
            {
                Id = retro.Id,
                EvaluacionId = retro.EvaluacionId,
                Texto = retro.Texto,
                FechaGeneracion = retro.FechaGeneracion,
                ModeloIA = retro.ModeloIA
            };

            return Ok(dto);
        }
    }
}

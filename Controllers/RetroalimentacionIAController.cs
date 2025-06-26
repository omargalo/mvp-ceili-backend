using CeiliApi.Data;
using CeiliApi.Models.DTOs;
using CeiliApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CeiliApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RetroalimentacionIAController : ControllerBase
    {
        private readonly CeiliDbContext _db;
        private readonly ChatGptService _chatService;

        public RetroalimentacionIAController(CeiliDbContext db, ChatGptService chatService)
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

            // (2) Construye el resumen por categoría y detecta el riesgo global
            var aspectos = JsonConvert.DeserializeObject<List<AspectoEvaluadoDto>>(evaluacion.AspectosJson ?? "[]") ?? new List<AspectoEvaluadoDto>();
            var resumen = aspectos
                .GroupBy(a => a.Categoria)
                .Select(g => $"{g.Key}: {ObtenRiesgoCategoria(g)}")
                .ToList();

            // Determina el riesgo global
            string nivelGlobal = ObtenerRiesgoGlobal(aspectos);

            // (3) Prompt dinámico según el riesgo global
            string instrucciones;
            if (nivelGlobal == "Bajo")
            {
                instrucciones = @"
- El alumno no presenta riesgos significativos.
- Escribe un mensaje breve y motivador dirigido al docente (máximo dos párrafos), reconociendo el buen trabajo realizado, y si acaso, una sugerencia de mejora general.";
            }
            else
            {
                instrucciones = @"
- El alumno presenta áreas de mejora.
- Da sugerencias claras en párrafos breves o una lista corta (máximo 5 puntos), según lo creas más útil.
- No repitas introducción, ve al grano y sé concreto, profesional y empático.";
            }

            string prompt = $@"
Eres un orientador escolar experto. Analiza el siguiente resumen de riesgos:
{string.Join("\n", resumen)}
Observaciones del docente: '{evaluacion.ObservacionDocente}'

INSTRUCCIONES:
{instrucciones}
";

            // (4) Llama a OpenAI
            var gptRespuesta = await _chatService.GetChatResponseAsync(prompt);

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

            // Helper para obtener el riesgo predominante por categoría
            string ObtenRiesgoCategoria(IGrouping<string, AspectoEvaluadoDto> grupo)
            {
                var cuenta = grupo.GroupBy(a => a.Riesgo)
                    .ToDictionary(g => g.Key, g => g.Count());
                if (cuenta.TryGetValue("Alto", out int alto) && alto > 0) return "Alto";
                if (cuenta.TryGetValue("Medio", out int medio) && medio > 0) return "Medio";
                return "Bajo";
            }

            // Helper para determinar el riesgo global (usa lógica similar a tu frontend)
            string ObtenerRiesgoGlobal(List<AspectoEvaluadoDto> aspectos)
            {
                int bajo = 0, medio = 0, alto = 0;
                foreach (var g in aspectos.GroupBy(a => a.Categoria))
                {
                    var cuenta = g.GroupBy(a => a.Riesgo).ToDictionary(x => x.Key, x => x.Count());
                    if (cuenta.TryGetValue("Alto", out int nAlto)) alto += nAlto;
                    if (cuenta.TryGetValue("Medio", out int nMedio)) medio += nMedio;
                    if (cuenta.TryGetValue("Bajo", out int nBajo)) bajo += nBajo;
                }
                if (alto >= medio && alto >= bajo && alto > 0) return "Alto";
                if (medio >= bajo && medio > 0) return "Medio";
                return "Bajo";
            }
        }
    }
}

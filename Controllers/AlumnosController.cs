using CeiliApi.Data;
using CeiliApi.Models.DTOs;
using CeiliApi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CeiliApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AlumnosController : ControllerBase
    {
        private readonly CeiliDbContext _db;

        public AlumnosController(CeiliDbContext db)
        {
            _db = db;
        }

        private int GetDocenteId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet]
        public async Task<IActionResult> GetMisAlumnos()
        {
            var docenteId = GetDocenteId();
            var alumnos = await _db.Alumnos
                .Where(a => a.DocenteId == docenteId)
                .ToListAsync();

            // Mapeo entidades → DTOs
            var dtos = alumnos.Select(a => new AlumnoDto
            {
                Id = a.Id,
                NombreCompleto = a.NombreCompleto,
                Edad = a.Edad,
                Sexo = a.Sexo
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> CrearAlumno([FromBody] CrearAlumnoDto dto)
        {
            var docenteId = GetDocenteId();
            var alumno = new Alumno
            {
                NombreCompleto = dto.NombreCompleto,
                Edad = dto.Edad,
                Sexo = dto.Sexo,
                DocenteId = docenteId
            };
            _db.Alumnos.Add(alumno);
            await _db.SaveChangesAsync();
            return Ok(new AlumnoDto
            {
                Id = alumno.Id,
                NombreCompleto = alumno.NombreCompleto,
                Edad = alumno.Edad,
                Sexo = alumno.Sexo
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarAlumno(int id, [FromBody] EditarAlumnoDto dto)
        {
            var docenteId = GetDocenteId();
            var alumno = await _db.Alumnos.FirstOrDefaultAsync(a => a.Id == id && a.DocenteId == docenteId);
            if (alumno == null) return NotFound();

            alumno.NombreCompleto = dto.NombreCompleto;
            alumno.Edad = dto.Edad;
            alumno.Sexo = dto.Sexo;
            await _db.SaveChangesAsync();
            return Ok(new AlumnoDto
            {
                Id = alumno.Id,
                NombreCompleto = alumno.NombreCompleto,
                Edad = alumno.Edad,
                Sexo = alumno.Sexo
            }); ;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAlumno(int id)
        {
            var docenteId = GetDocenteId();
            var alumno = await _db.Alumnos.FirstOrDefaultAsync(a => a.Id == id && a.DocenteId == docenteId);
            if (alumno == null) return NotFound();

            _db.Alumnos.Remove(alumno);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}

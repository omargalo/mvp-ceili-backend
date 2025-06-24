using CeiliApi.Data;
using CeiliApi.Models.DTOs;
using CeiliApi.Models.Entities;
using CeiliApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CeiliApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CeiliDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(CeiliDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistroDocenteDto dto)
        {
            if (_db.Docentes.Any(d => d.Email == dto.Email))
                return Conflict("El correo ya está registrado.");

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(dto.Password, salt);

            var docente = new Docente
            {
                NombreCompleto = dto.NombreCompleto,
                Grupo = dto.Grupo,
                Email = dto.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _db.Docentes.Add(docente);
            await _db.SaveChangesAsync();

            return Ok("Registro exitoso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDocenteDto dto)
        {
            var docente = await _db.Docentes.FirstOrDefaultAsync(d => d.Email == dto.Email);
            if (docente == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            var hash = PasswordHelper.HashPassword(dto.Password, docente.PasswordSalt);
            if (docente.PasswordHash != hash)
                return Unauthorized("Correo o contraseña incorrectos.");

            var token = JwtHelper.GenerateJwtToken(docente.Id, docente.Email, _config);
            return Ok(new
            {
                docente = new DocenteDto
                {
                    Id = docente.Id,
                    NombreCompleto = docente.NombreCompleto,
                    Grupo = docente.Grupo,
                    Email = docente.Email
                },
                token
            });
        }
    }
}

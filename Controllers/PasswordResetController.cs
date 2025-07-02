using CeiliApi.Models.DTOs;
using CeiliApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CeiliApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly PasswordResetService _resetService;

        public PasswordResetController(PasswordResetService resetService)
        {
            _resetService = resetService;
        }

        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarReset([FromBody] PasswordResetRequestDto dto)
        {
            // Por seguridad, siempre responde éxito aunque el mail no exista
            await _resetService.SendPasswordResetTokenAsync(dto.Email);
            return Ok(new { message = "Si el correo está registrado, recibirás un enlace para restablecer la contraseña." });
        }

        [HttpGet("validar")]
        public async Task<IActionResult> ValidarToken([FromQuery] string token)
        {
            var esValido = await _resetService.EsTokenValidoAsync(token);
            return Ok(new { valido = esValido });
        }

        [HttpPost("cambiar")]
        public async Task<IActionResult> CambiarPassword([FromBody] PasswordResetDto dto)
        {
            var ok = await _resetService.ResetPasswordAsync(dto.Token, dto.NuevoPassword);
            if (!ok)
                return BadRequest("El token es inválido o ha expirado.");
            return Ok(new { message = "Contraseña cambiada correctamente." });
        }

    }
}

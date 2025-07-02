using CeiliApi.Business;
using CeiliApi.Models.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace CeiliApi.Services
{
    public class PasswordResetService
    {
        private readonly PasswordReset _passwordReset;
        private readonly IConfiguration _config;

        public PasswordResetService(PasswordReset passwordReset, IConfiguration config)
        {
            _passwordReset = passwordReset;
            _config = config;
        }

        public async Task<bool> SendPasswordResetTokenAsync(string email)
        {
            var reset = await _passwordReset.GenerarTokenResetAsync(email);
            if (reset == null)
                return false;

            // Construye el enlace de recuperación
            var frontendUrl = _config["PasswordReset:FrontendUrl"];
            var link = $"{frontendUrl}?token={Uri.EscapeDataString(reset.Token)}";

            // Envía el correo
            await SendMailAsync(reset.Docente.Email, "Recupera tu contraseña", $@"
Hola {reset.Docente.NombreCompleto},
Hemos recibido una solicitud para restablecer tu contraseña.
Haz clic en el siguiente enlace para crear una nueva contraseña (válido 30 minutos):

{link}

Si no solicitaste este cambio, ignora este mensaje.");

            return true;
        }

        private async Task SendMailAsync(string to, string subject, string body)
        {
            var smtpHost = _config["Smtp:Host"] ?? throw new InvalidOperationException("No SMTP host configured.");
            var smtpPortStr = _config["Smtp:Port"] ?? throw new InvalidOperationException("No SMTP port configured.");
            var smtpUser = _config["Smtp:User"] ?? throw new InvalidOperationException("No SMTP user configured.");
            var smtpPass = _config["Smtp:Password"] ?? throw new InvalidOperationException("No SMTP password configured.");

            // int.Parse puede lanzar excepción, por eso validamos arriba que no sea null.
            var smtpPort = int.Parse(smtpPortStr);

            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("El destinatario (to) no puede estar vacío.", nameof(to));

            var mail = new MailMessage(smtpUser, to, subject ?? "", body ?? "");

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            await client.SendMailAsync(mail);
        }


        public Task<bool> EsTokenValidoAsync(string token) => _passwordReset.EsTokenValidoAsync(token);

        public Task<bool> ResetPasswordAsync(string token, string nuevoPassword) => _passwordReset.ResetPasswordAsync(token, nuevoPassword);
    }
}

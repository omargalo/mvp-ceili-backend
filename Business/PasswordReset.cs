using CeiliApi.Data;
using CeiliApi.Models.Entities;
using CeiliApi.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CeiliApi.Business
{
    public class PasswordReset
    {
        private readonly CeiliDbContext _db;

        public PasswordReset(CeiliDbContext db)
        {
            _db = db;
        }

        public async Task<PasswordResetToken?> GenerarTokenResetAsync(string email)
        {
            var docente = await _db.Docentes.FirstOrDefaultAsync(d => d.Email == email);
            if (docente == null)
                return null;

            // Elimina tokens previos no usados
            var tokensAntiguos = _db.PasswordResetTokens
                .Where(t => t.DocenteId == docente.Id && (!t.Usado || t.FechaExpiracion < DateTime.UtcNow));
            _db.PasswordResetTokens.RemoveRange(tokensAntiguos);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            var expiration = DateTime.UtcNow.AddMinutes(30);

            var reset = new PasswordResetToken
            {
                DocenteId = docente.Id,
                Token = token,
                FechaExpiracion = expiration,
                Usado = false
            };
            _db.PasswordResetTokens.Add(reset);
            await _db.SaveChangesAsync();

            return reset;
        }

        public async Task<bool> EsTokenValidoAsync(string token)
        {
            return await _db.PasswordResetTokens.AnyAsync(r =>
                r.Token == token && !r.Usado && r.FechaExpiracion >= DateTime.UtcNow);
        }

        public async Task<bool> ResetPasswordAsync(string token, string NuevoPassword)
        {
            var reset = await _db.PasswordResetTokens
                .Include(r => r.Docente)
                .FirstOrDefaultAsync(r =>
                    r.Token == token &&
                    !r.Usado &&
                    r.FechaExpiracion >= DateTime.UtcNow);

            if (reset == null || reset.Docente == null)
                return false;

            var nuevoSalt = PasswordHelper.GenerateSalt();
            var nuevoHash = PasswordHelper.HashPassword(NuevoPassword, nuevoSalt);

            reset.Docente.PasswordSalt = nuevoSalt;
            reset.Docente.PasswordHash = nuevoHash;
            reset.Usado = true;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}

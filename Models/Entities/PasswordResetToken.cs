using System;

namespace CeiliApi.Models.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public Docente Docente { get; set; } = null!;
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; } = false;
    }
}

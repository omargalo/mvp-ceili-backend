using System.Collections.Generic;

namespace CeiliApi.Models.Entities
{
    public class Docente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Grupo { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string PasswordSalt { get; set; } = "";
        public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
        public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
    }

}

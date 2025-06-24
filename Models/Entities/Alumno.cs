using System.Collections.Generic;

namespace CeiliApi.Models.Entities
{
    public class Alumno
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = "";
        public int Edad { get; set; }
        public string Sexo { get; set; } = "";
        public int DocenteId { get; set; }
        public Docente Docente { get; set; } = null!;
        public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
    }

}

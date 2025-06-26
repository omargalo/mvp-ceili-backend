using System;

namespace CeiliApi.Models.Entities
{
    public class Evaluacion
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set; } = null!;
        public int DocenteId { get; set; }
        public Docente Docente { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string AspectosJson { get; set; } = "";
        public string ObservacionDocente { get; set; } = "";
        public RetroalimentacionIA? Retroalimentacion { get; set; }

    }

}

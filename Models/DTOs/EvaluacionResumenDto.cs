using System;
using System.Collections.Generic;

namespace CeiliApi.Models.DTOs
{
    public class EvaluacionResumenDto
    {
        public int Id { get; set; }
        public string AlumnoNombre { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string ObservacionDocente { get; set; } = "";
        public List<AspectoEvaluadoDto> Aspectos { get; set; } = new();
    }
}

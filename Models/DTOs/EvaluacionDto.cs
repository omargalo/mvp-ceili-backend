using System.Collections.Generic;
using System;

namespace CeiliApi.Models.DTOs
{
    public class EvaluacionDto
    {
        public string Alumno { get; set; } = "";
        public string Grupo { get; set; } = "";
        public DateTime Fecha { get; set; }
        public List<AspectoEvaluadoDto> Aspectos { get; set; } = new();
        public string ObservacionDocente { get; set; } = "";
    }

    public class AspectoEvaluadoDto
    {
        public string Nombre { get; set; } = "";
        public string Riesgo { get; set; } = ""; // "Bajo", "Medio", "Alto"
    }
}

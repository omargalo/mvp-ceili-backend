using System.Collections.Generic;
using System;

namespace CeiliApi.Models.DTOs
{
    public class EvaluacionDto
    {
        public DateTime Fecha { get; set; }
        public List<AspectoEvaluadoDto> Aspectos { get; set; } = new();
        public string ObservacionDocente { get; set; } = "";
        public int AlumnoId { get; set; }
        public int DocenteId { get; set; }
    }

    public class AspectoEvaluadoDto
    {
        public string Categoria { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Riesgo { get; set; } = "";
    }
}

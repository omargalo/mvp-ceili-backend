using System;

namespace CeiliApi.Models.Entities
{
    public class RetroalimentacionIA
    {
        public int Id { get; set; }

        // FK a Evaluacion
        public int EvaluacionId { get; set; }
        public Evaluacion Evaluacion { get; set; } = null!;

        // Texto generado por la IA
        public string Texto { get; set; } = "";

        public DateTime FechaGeneracion { get; set; }

        // Modelo GPT utilizado
        public string? ModeloIA { get; set; }
    }
}

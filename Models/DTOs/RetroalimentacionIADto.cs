using System;

namespace CeiliApi.Models.DTOs
{
    public class RetroalimentacionIADto
    {
        public int Id { get; set; }
        public int EvaluacionId { get; set; }
        public string Texto { get; set; } = "";
        public DateTime FechaGeneracion { get; set; }
        public string? ModeloIA { get; set; }
    }
}

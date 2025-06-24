namespace CeiliApi.Models.DTOs
{
    public class AlumnoDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = "";
        public int Edad { get; set; }
        public string Sexo { get; set; } = "";
    }
}

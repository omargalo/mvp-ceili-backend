namespace CeiliApi.Models.DTOs
{
    public class PasswordResetDto
    {
        public string Token { get; set; } = "";
        public string NuevoPassword { get; set; } = "";
    }
}

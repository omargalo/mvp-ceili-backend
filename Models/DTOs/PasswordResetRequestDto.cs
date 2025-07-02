using System.ComponentModel.DataAnnotations;

namespace CeiliApi.Models.DTOs
{
    public class PasswordResetRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }
}

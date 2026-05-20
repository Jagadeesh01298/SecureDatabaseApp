using System.ComponentModel.DataAnnotations;

namespace SecureDatabaseApp.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Encrypted sensitive data
        [Required]
        public string EncryptedFinancialInfo { get; set; } = string.Empty;

        // HMAC to verify data integrity
        [Required]
        public string FinancialInfoHmac { get; set; } = string.Empty;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBBRARY_MANAGER.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id_User { get; set; }

        [Required, MaxLength(100)]
        public string Name_User { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        [Required, MaxLength(256)]
        [Column("Password_Hash")]
        public string HashedPassword { get; protected set; } = null!;

        [NotMapped]
        public string Password
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Le mot de passe ne peut pas être vide.");
                HashedPassword = HashPassword(value);
            }
        }

        [MaxLength(200)]
        public string? Adresse { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Adresse_Mail { get; set; }

        [MaxLength(20)]
        public string? Num_Telephone { get; set; }

        public DateTime Date_Creation { get; protected set; }

        [NotMapped]
        public int? Age => BirthDate.HasValue
            ? DateTime.Now.Year - BirthDate.Value.Year - (DateTime.Now.DayOfYear < BirthDate.Value.DayOfYear ? 1 : 0)
            : null;

        public User()
        {
            Date_Creation = DateTime.Now;
        }

        public static string HashPassword(string password)
        {
            // IMPORTANT: En production, utilisez BCrypt.Net-Next
            // Install-Package BCrypt.Net-Next
            // return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

            // Version temporaire avec SHA256 (à remplacer!)
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + "SALT_UNIQUE_PAR_APP");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string inputPassword)
        {
            // Avec BCrypt: return BCrypt.Net.BCrypt.Verify(inputPassword, HashedPassword);
            var hashedInput = HashPassword(inputPassword);
            return HashedPassword == hashedInput;
        }
    }
}
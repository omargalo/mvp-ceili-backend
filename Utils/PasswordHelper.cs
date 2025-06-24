using System;
using System.Text;
using System.Security.Cryptography;

namespace CeiliApi.Utils
{
    public static class PasswordHelper
    {
        public static string GenerateSalt(int size = 16)
        {
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

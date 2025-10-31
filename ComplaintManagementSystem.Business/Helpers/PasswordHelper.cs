using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ComplaintManagementSystem.Business.Helpers
{
    public class PasswordHelper
    {
        private readonly string _saltKey;

        public PasswordHelper(IConfiguration configuration)
        {
            _saltKey = configuration["UserEncryption:SaltKey"];

            if (string.IsNullOrWhiteSpace(_saltKey))
                throw new InvalidOperationException("SaltKey is missing or empty in appsettings.json (AppSettings:SaltKey).");
        }

        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string ComputeHmac(string password)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_saltKey);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var computedHash = ComputeHmac(enteredPassword);
            return storedHash == computedHash;
        }
    }
}

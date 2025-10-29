using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.Helpers
{
    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string ComputeHmac(string salt, string password)
        {
            var keyBytes = Encoding.UTF8.GetBytes(salt);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var computedHash = ComputeHmac(storedSalt, enteredPassword);
            return storedHash == computedHash;
        }

        public static string EncrypthPassword(string enteredPassword, string storedSalt)
        {
            var computedHash = ComputeHmac(storedSalt, enteredPassword);
            return computedHash;
        }
    }
}

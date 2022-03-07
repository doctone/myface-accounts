using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyFace.Helpers
{
    public class PasswordHasher
    {
        public string Password { get; set; }
        public string Salt { get; set; }
        public PasswordHasher(string password)
        {
            byte[] s = GenerateSalt();
            Password = HashPassword(password, s);
            Salt = Convert.ToBase64String(s);
        }

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
        }
        public string HashPassword(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
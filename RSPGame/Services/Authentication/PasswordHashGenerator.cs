using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace RSPGame.Services.Authentication
{
    public class PasswordHashGenerator
    {
        private static readonly byte[] Salt;

        static PasswordHashGenerator()
        {
            Salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(Salt);
        }
        
        public string GenerateHash(string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        
            return hashed;
        }

        public bool AreEqual(string password, string hashedPassword)
        {
            var newHashedPin = GenerateHash(password);
            
            return newHashedPin.Equals(hashedPassword); 
        }
    }
}
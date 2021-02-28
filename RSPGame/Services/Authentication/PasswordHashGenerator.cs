using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace RSPGame.Services.Authentication
{
    public class PasswordHashGenerator
    {
        public string GenerateHash(string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[128 / 8],
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
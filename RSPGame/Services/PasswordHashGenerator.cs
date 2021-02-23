using System;
using System.Security.Cryptography;
using System.Text;

namespace RSPGame.Services
{
    public class PasswordHashGenerator
    {
        private static readonly string Salt;

        static PasswordHashGenerator()
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[16];
            
            rng.GetBytes(buff);
            
            Salt = Convert.ToBase64String(buff);
        }

        public string GenerateHash(string password)
        { 
            var bytes = Encoding.UTF8.GetBytes(password + Salt);
            
            var sHa256ManagedString = new SHA256Managed();
            
            //todo change to async
            var hash = sHa256ManagedString.ComputeHash(bytes);
            
            return Convert.ToBase64String(hash);
        }

        public bool AreEqual(string password, string hashedPassword)
        {
            var newHashedPin = GenerateHash(password);
            
            return newHashedPin.Equals(hashedPassword); 
        }
    }
}
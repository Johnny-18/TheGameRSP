using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> GenerateHash(string password)
        { 
            var bytes = Encoding.UTF8.GetBytes(password + Salt);

            var stream = new MemoryStream();
            stream.Write(bytes);
            
            var sHa256ManagedString = new SHA256Managed();
            
            var hash = await sHa256ManagedString.ComputeHashAsync(stream);
            
            return Convert.ToBase64String(hash);
        }

        public async Task<bool> AreEqual(string password, string hashedPassword)
        {
            var newHashedPin = await GenerateHash(password);
            
            return newHashedPin.Equals(hashedPassword); 
        }
    }
}
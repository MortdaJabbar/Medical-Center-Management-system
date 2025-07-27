using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;

namespace MCMSAPI.Helper
{
    public static class PasswordHelper
    {
        
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }

}

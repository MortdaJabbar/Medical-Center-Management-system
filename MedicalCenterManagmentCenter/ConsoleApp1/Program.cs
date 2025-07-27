using BCrypt.Net;

namespace ConsoleApp1
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
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string hash = PasswordHelper.HashPassword("123");
            string password = "$2a$11$hfm0043PBg8skXZVReHW6uhFowsGmNDAQcanDmYhQ/NlxVZeYsSnW";

            Console.WriteLine(hash);
 
            Console.WriteLine(PasswordHelper.VerifyPassword("123", password));
            Console.ReadKey();
        }
    }
}

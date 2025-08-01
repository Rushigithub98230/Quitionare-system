using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("Password hashes for testing:");
        Console.WriteLine($"admin123: {HashPassword("admin123")}");
        Console.WriteLine($"user123: {HashPassword("user123")}");
    }

    static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
} 
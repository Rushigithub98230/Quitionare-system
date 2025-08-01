using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string password = "admin123";
        string hash = HashPassword(password);
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
        
        // Test verification
        bool isValid = VerifyPassword(password, hash);
        Console.WriteLine($"Verification: {isValid}");
    }
    
    static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
    
    static bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
} 
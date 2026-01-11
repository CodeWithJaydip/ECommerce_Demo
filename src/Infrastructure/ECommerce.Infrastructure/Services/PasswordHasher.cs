using BCrypt.Net;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Service for password hashing using BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hash a password using BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}

/// <summary>
/// Interface for password hashing
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

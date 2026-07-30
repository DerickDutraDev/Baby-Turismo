using BabyTurismo.Application.Common.Interfaces;

namespace BabyTurismo.Infrastructure.Services;

public sealed class BcryptPasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 11);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}

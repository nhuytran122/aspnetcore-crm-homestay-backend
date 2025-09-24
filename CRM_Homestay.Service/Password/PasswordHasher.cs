using CRM_Homestay.Contract.Password;
using CRM_Homestay.Entity.Users;
using Microsoft.AspNetCore.Identity;

namespace CRM_Homestay.Service.Password;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _userHasher = new();

    public string Hash(User user, string password)
    {
        return _userHasher.HashPassword(user, password);
    }

    public bool Verify(User user, string password, string hashedPassword)
    {
        var result = _userHasher.VerifyHashedPassword(user, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}

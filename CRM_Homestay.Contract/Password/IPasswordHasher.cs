using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Contract.Password;

public interface IPasswordHasher
{
    string Hash(User user, string password);
    bool Verify(User user, string password, string hashedPassword);
}

using CRM_Homestay.Entity.Customers;
using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Contract.Password;

public interface IPasswordHasher
{
    string Hash(CustomerAccount user, string password);
    bool Verify(CustomerAccount user, string password, string hashedPassword);

    string Hash(User user, string password);
    bool Verify(User user, string password, string hashedPassword);
}

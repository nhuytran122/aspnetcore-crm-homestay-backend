
namespace CRM_Homestay.Contract.Users
{
    public class BasicInfoUserDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
        public string? RoleCode { get; set; }
    }
}

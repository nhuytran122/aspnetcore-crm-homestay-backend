
namespace CRM_Homestay.Contract.Customers
{
    public class CustomerInfoDto
    {
        public Guid Id { get; set; }
        public string? CustomerGroupName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

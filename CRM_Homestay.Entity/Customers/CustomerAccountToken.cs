using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.Customers
{
    public class CustomerAccountToken : BaseEntity
    {
        public Guid CustomerAccountId { get; set; }
        public string Token { get; set; } = "";

        public CustomerAccount? CustomerAccount { get; set; }
    }
}

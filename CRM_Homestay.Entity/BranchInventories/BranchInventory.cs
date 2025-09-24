using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Entity.Products;

namespace CRM_Homestay.Entity.BranchInventories
{
    public class BranchInventory
    {
        public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityOnHand { get; set; }
        public Branch? Branch { get; set; }
        public Product? Product { get; set; }
    }
}

using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Products;

namespace CRM_Homestay.Entity.ProductCategories
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int TypeCategory { get; set; }
        public string? NormalizeFullInfo { get; set; }

        public List<Product>? Products { get; set; }

    }
}

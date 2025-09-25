using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BranchInventories;
using CRM_Homestay.Entity.ImportProductDetails;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.ProductCategories;

namespace CRM_Homestay.Entity.Products
{
    public class Product : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? MediaId { get; set; }
        public bool Enable { get; set; }
        public string? Description { get; set; }
        public string? NormalizeFullInfo { get; set; }

        public ProductCategory? ProductCategory { get; set; }
        public List<ImportProductDetail>? ImportProductDetails { get; set; }
        public List<BranchInventory>? BranchInventories { get; set; }
        public BaseMedia? Media { get; set; }
    }
}

using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Entity.ImportProductDetails;
using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Entity.ImportProducts
{
    public class ImportProduct : BaseEntity, IAuditable
    {
        public string ImportCode { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime DeletedAt { get; set; }
        public string? Note { get; set; }
        public Guid? PaymentImgId { get; set; }
        public Branch? Branch { get; set; }
        public User? User { get; set; }

        public List<ImportProductDetail>? ImportProductDetails { get; set; }
        public BaseMedia? PaymentImg { get; set; }
    }
}

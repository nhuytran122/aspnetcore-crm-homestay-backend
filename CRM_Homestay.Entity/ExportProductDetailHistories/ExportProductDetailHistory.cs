using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Entity.ImportProductDetails;

namespace CRM_Homestay.Entity.ExportProductDetailHistories
{
    public class ExportProductDetailHistory : BaseEntity, IAuditable
    {
        public string ExportCode { get; set; } = string.Empty;
        public Guid ImportProductDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public ImportProductDetail? ImportProductDetail { get; set; }
        public BookingService? BookingService { get; set; }
    }
}

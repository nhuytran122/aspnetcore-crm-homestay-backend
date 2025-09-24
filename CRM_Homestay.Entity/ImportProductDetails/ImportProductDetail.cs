using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.ExportProductDetailHistories;
using CRM_Homestay.Entity.ImportProducts;
using CRM_Homestay.Entity.Products;

namespace CRM_Homestay.Entity.ImportProductDetails
{
    public class ImportProductDetail : IAuditable
    {
        public Guid Id { get; set; }
        public Guid ImportProductId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; } = 0;
        public decimal SubPrice { get; set; } = 0;
        /// <summary>
        /// Số tiền đã trả
        /// </summary>
        public decimal? Paid { get; set; } = 0;
        public ImportProduct? ImportProduct { get; set; }
        public Product? Product { get; set; }
        public List<ExportProductDetailHistory>? ExportProductDetailHistories { get; set; }
    }
}

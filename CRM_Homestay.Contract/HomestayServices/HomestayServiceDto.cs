namespace CRM_Homestay.Contract.HomestayServices
{
    public class HomestayServiceDto
    {
        public string? Name { get; set; } 
        public decimal Price { get; set; }
        public bool IsPrepaid { get; set; }
        public bool HasInventory { get; set; }
        public string? Description { get; set; }
    }
}
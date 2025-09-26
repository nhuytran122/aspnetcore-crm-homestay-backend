namespace CRM_Homestay.Contract.RoomAmenities
{
    public class RoomAmenityDto
    {
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public Guid BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid AmenityId { get; set; }
        public string? AmenityName { get; set; }
        public string? Type { get; set; }
        public int Quantity { get; set; }
    }
}
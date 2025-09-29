using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Rooms
{
    public class RoomDto : BaseEntityDto
    {
        public int RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public string? MediaUrl { get; set; }
    }
}
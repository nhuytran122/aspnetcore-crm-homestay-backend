using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Medias;
using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Contract.RoomPricings;

namespace CRM_Homestay.Contract.Rooms
{
    public class RoomDetailDto : BaseEntityDto
    {
        public int RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public List<RoomAmenityDto>? RoomAmenities { get; set; }
        public List<RoomPricingDto>? Prices { get; set; }
        // public List<string>? ImagePaths { get; set; }
        public List<MediaInfoDto>? Images { get; set; }
    }
}
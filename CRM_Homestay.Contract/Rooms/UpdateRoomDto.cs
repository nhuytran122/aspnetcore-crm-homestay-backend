using CRM_Homestay.Contract.RoomAmenities;
using Microsoft.AspNetCore.Http;

namespace CRM_Homestay.Contract.Rooms
{
    public class UpdateRoomDto
    {
        public int? RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid BranchId { get; set; }
        public Guid RoomTypeId { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<UpdateAmenityForRoomDto>? RoomAmenities { get; set; }
    }

    public class UpdateAmenityForRoomDto
    {
        public Guid? Id { get; set; }
        public Guid AmenityId { get; set; }
        public int? Quantity { get; set; }
    }


}
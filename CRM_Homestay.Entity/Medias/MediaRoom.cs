using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Entity.Medias
{
    public class MediaRoom
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid MediaId { get; set; }

        public Room? Room { get; set; }
        public BaseMedia? Media { get; set; }
    }
}

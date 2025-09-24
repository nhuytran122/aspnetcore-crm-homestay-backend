using CRM_Homestay.Entity.Medias;
using CRM_Homestay.Entity.Reviews;
using CRM_Homestay.Entity.Rooms;

namespace CRM_Homestay.Entity.Media
{
    public class MediaReview
    {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public Guid MediaId { get; set; }

        public Review? Review { get; set; }
        public BaseMedia? Media { get; set; }
    }
}

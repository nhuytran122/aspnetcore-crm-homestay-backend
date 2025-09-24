
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.Media;

namespace CRM_Homestay.Entity.Medias
{
    public class BaseMedia : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string FileNameUpload { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        public List<MediaRoom>? MediaRooms { get; set; }
        public List<MediaReview>? MediaReviews { get; set; }
        
    }
}

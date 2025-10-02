using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Reviews
{
    public class ReviewDto : BaseEntityDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
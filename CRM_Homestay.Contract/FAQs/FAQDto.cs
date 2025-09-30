using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.FAQs
{
    public class FAQDto : BaseEntityDto
    {
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool IsActive { get; set; }
    }
}
using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.FAQs
{
    public class FAQFilterDto : FilterBase, IPagedResultRequest
    {
        public bool? IsActive { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

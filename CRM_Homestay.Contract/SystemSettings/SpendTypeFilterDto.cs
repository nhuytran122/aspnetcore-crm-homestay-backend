using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.SystemSettings
{
    public class SpendTypeFilterDto : FilterBase
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

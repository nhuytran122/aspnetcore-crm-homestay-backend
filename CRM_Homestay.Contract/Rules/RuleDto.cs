using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Rules
{
    public class RuleDto : BaseEntityDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
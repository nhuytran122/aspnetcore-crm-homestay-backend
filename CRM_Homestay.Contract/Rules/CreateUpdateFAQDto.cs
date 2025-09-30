namespace CRM_Homestay.Contract.Rules
{
    public class CreateUpdateRuleDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
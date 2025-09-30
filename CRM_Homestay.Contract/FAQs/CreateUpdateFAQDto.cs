namespace CRM_Homestay.Contract.FAQs
{
    public class CreateUpdateFAQDto
    {
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
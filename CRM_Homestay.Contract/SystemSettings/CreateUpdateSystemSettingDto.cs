
namespace CRM_Homestay.Contract.SystemSettings
{
    public class CreateUpdateSystemSettingDto
    {
        public Guid? Id { get; set; }
        public string ConfigValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Sort { get; set; } = 0;
    }
}

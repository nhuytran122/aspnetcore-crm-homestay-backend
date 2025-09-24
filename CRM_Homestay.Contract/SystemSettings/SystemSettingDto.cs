
namespace CRM_Homestay.Contract.SystemSettings
{
    public class SystemSettingDto
    {
        public Guid Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Sort { get; set; }
    }

    public class SystemSettingZaloDto
    {
        public string Label { get; set; } = string.Empty;
        public string? ConfigKey { get; set; }
        public string? ConfigValue { get; set; }
    }

    public class UpdateSystemSettingZaloDto
    {
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
    }
}

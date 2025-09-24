using CRM_Homestay.Entity.Bases;

namespace CRM_Homestay.Entity.SystemSettings
{
    public class SystemSetting : BaseEntity
    {
        public string SystemName { get; set; } = string.Empty;
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Sort { get; set; } = 0;
    }
}

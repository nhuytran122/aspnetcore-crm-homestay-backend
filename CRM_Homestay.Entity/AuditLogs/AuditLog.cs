using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Entity.AuditLogs
{
    public class AuditLog
    {
        public long Id { get; set; }
        public Guid AuditRecordId { get; set; }
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
        public string? TableName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        public User? User { get; set; }
    }
}

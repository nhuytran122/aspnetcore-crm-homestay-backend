namespace CRM_Homestay.Entity.Bases;

public class SuperEntity : BaseEntity
{
    public DateTime DeletionTime { get; set; } = DateTime.UtcNow;
}
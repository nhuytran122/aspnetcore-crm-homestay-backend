namespace CRM_Homestay.Contract.Bases;

public class SuperEntityDto : BaseEntityDto
{
    public DateTime LastModificationTime { get; set; }
    public Guid LastModifierId { get; set; }
    public DateTime DeletionTime { get; set; } = DateTime.UtcNow;
    public Guid CreatorId { get; set; }
}
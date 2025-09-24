namespace CRM_Homestay.Contract.Bases;

public class BaseEntityDto
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
}
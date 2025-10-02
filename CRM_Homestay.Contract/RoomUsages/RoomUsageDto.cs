namespace CRM_Homestay.Contract.RoomUsages
{
    public class RoomUsageDto
    {
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

}
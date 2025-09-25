using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.RoomPricings
{
    public interface IRoomPricingService
    {
        Task<RoomPricingDto> CreateAsync(CreateRoomPricingDto input);
        Task<RoomPricingDto> GetByIdAsync(Guid id);
        Task<RoomPricingDto> UpdateAsync(Guid id, UpdateRoomPricingDto input);
        Task<List<RoomPricingDto>> GetPricingByRoomTypeId(Guid typeId);
        Task<List<RoomPricingDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
    }
}

using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.RoomTypes
{
    public interface IRoomTypeService
    {
        Task<RoomTypeDto> CreateAsync(CreateUpdateRoomTypeDto input);
        Task<RoomTypeDto> GetByIdAsync(Guid id);
        Task<RoomTypeDto> UpdateAsync(Guid id, CreateUpdateRoomTypeDto input);
        Task<PagedResultDto<RoomTypeDto>> GetPagingWithFilterAsync(RoomTypeFilterDto input);
        Task<List<RoomTypeDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
    }
}

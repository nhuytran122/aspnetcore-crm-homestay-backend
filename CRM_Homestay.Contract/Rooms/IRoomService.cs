using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Rooms
{
    public interface IRoomService
    {
        Task<RoomDto> CreateAsync(CreateRoomDto input);
        Task<RoomDetailDto> GetByIdAsync(Guid id);
        Task<RoomDto> UpdateAsync(Guid id, UpdateRoomDto input);
        Task<PagedResultDto<RoomDto>> GetPagingWithFilterAsync(RoomFilterDto input);
        Task<List<RoomDto>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task<List<RoomDto>> GetAllActiveAsync();
        Task<List<RoomDto>> GetByBranchAsync(Guid branchId);
        Task<List<RoomDto>> GetByRoomTypeAsync(Guid roomTypeId);
        Task<List<RoomDto>> GetByBranchAndRoomTypeAsync(RoomFilterDto input);
    }
}

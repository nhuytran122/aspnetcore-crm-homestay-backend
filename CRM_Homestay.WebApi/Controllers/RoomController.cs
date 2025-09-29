using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Rooms;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// RoomController
    /// </summary>
    [Route("api/rooms")]
    [ApiController]
    [Authorize]
    public class RoomController : BaseController
    {
        private readonly IRoomService _roomService;
        /// <summary>
        /// RoomController init
        /// </summary>
        /// <param name="roomService"></param>
        /// <param name="httpContextAccessor"></param>
        public RoomController(IRoomService roomService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Tạo phòng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<RoomDto> Create([FromForm] CreateRoomDto input)
        {
            return await _roomService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin phòng theo ID phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<RoomDetailDto> GetById(Guid id)
        {
            return await _roomService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách phòng kết hợp filter và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<PagedResultDto<RoomDto>> GetWithFilter([FromQuery] RoomFilterDto request)
        {
            return await _roomService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin phòng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<RoomDto> Update(Guid id, [FromForm] UpdateRoomDto input)
        {
            return await _roomService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phòng đang hoạt động
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        public async Task<List<RoomDto>> GetAllActive()
        {
            return await _roomService.GetAllActiveAsync();
        }

        /// <summary>
        /// Xóa phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _roomService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ phòng ( hoạt động + không + xóa mềm (chỉ lấy những phòng xóa trong vòng 3 tháng))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<RoomDto>> GetAll()
        {
            return await _roomService.GetAllAsync();
        }

        /// <summary>
        /// Lọc phòng theo chi nhánh
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        [HttpGet("by-branch/{branchId}")]
        public async Task<List<RoomDto>> GetByBranch(Guid branchId)
        {
            return await _roomService.GetByBranchAsync(branchId);
        }

        /// <summary>
        /// Lọc phòng theo loại phòng
        /// </summary>
        /// <param name="roomTypeId"></param>
        /// <returns></returns>
        [HttpGet("by-room-type/{roomTypeId}")]
        public async Task<List<RoomDto>> GetByRoomType(Guid roomTypeId)
        {
            return await _roomService.GetByRoomTypeAsync(roomTypeId);
        }

        /// <summary>
        /// Lọc phòng theo chi nhánh và loại phòng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("by-branch-and-type")]
        public async Task<List<RoomDto>> GetByBranchAndRoomType([FromQuery] RoomFilterDto input)
        {
            return await _roomService.GetByBranchAndRoomTypeAsync(input);
        }
    }
}

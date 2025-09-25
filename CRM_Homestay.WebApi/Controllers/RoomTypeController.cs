using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Contract.RoomTypes;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// RoomTypeController
    /// </summary>
    [Route("api/room-types")]
    [ApiController]
    [Authorize]
    public class RoomTypeController : BaseController
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IRoomPricingService _pricingService;
        /// <summary>
        /// RoomTypeController init
        /// </summary>
        /// <param name="roomTypeService"></param>
        /// /// <param name="pricingService"></param>
        /// <param name="httpContextAccessor"></param>
        public RoomTypeController(IRoomTypeService roomTypeService, IRoomPricingService pricingService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roomTypeService = roomTypeService;
            _pricingService = pricingService;
        }

        /// <summary>
        /// Tạo loại phòng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<RoomTypeDto> Create([FromForm] CreateUpdateRoomTypeDto input)
        {
            return await _roomTypeService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin loại phòng theo ID loại phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("{id}")]
        public async Task<RoomTypeDto> GetById(Guid id)
        {
            return await _roomTypeService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách loại phòng kết hợp filter(tên/sdt, status, startDate, endDate) và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("search")]
        public async Task<PagedResultDto<RoomTypeDto>> GetWithFilter([FromQuery] RoomTypeFilterDto request)
        {
            return await _roomTypeService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin loại phòng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<RoomTypeDto> Update(Guid id, [FromForm] CreateUpdateRoomTypeDto input)
        {
            return await _roomTypeService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa loại phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _roomTypeService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ loại phòng (hoạt động + không + xóa mềm (chỉ lấy những loại phòng xóa trong vòng 3 tháng))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<RoomTypeDto>> GetAll()
        {
            return await _roomTypeService.GetAllAsync();
        }

        /// <summary>
        /// Lấy toàn bộ giá của loại phòng (hoạt động + không + xóa mềm (chỉ lấy những giá phòng xóa trong vòng 3 tháng))
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("room-pricings/{id}")]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<RoomPricingDto>> GetByRoomTypeId(Guid id)
        {
            return await _pricingService.GetPricingByRoomTypeId(id);
        }
    }
}

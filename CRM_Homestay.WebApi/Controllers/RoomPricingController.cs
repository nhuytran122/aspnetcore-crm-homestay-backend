using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// RoomPricingController
    /// </summary>
    [Route("api/room-pricings")]
    [ApiController]
    [Authorize]
    public class RoomPricingController : BaseController
    {
        private readonly IRoomPricingService _pricingService;
        /// <summary>
        /// RoomPricingController init
        /// </summary>
        /// <param name="roomTypeService"></param>
        /// <param name="httpContextAccessor"></param>
        public RoomPricingController(IRoomPricingService roomTypeService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _pricingService = roomTypeService;
        }

        /// <summary>
        /// Tạo giá phòng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<RoomPricingDto> Create([FromBody] CreateRoomPricingDto input)
        {
            return await _pricingService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin giá phòng theo ID giá phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("{id}")]
        public async Task<RoomPricingDto> GetById(Guid id)
        {
            return await _pricingService.GetByIdAsync(id);
        }

        /// <summary>
        /// Cập nhật thông tin giá phòng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<RoomPricingDto> Update(Guid id, [FromBody] UpdateRoomPricingDto input)
        {
            return await _pricingService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa giá phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _pricingService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ giá phòng (hoạt động + không + xóa mềm (chỉ lấy những giá phòng xóa trong vòng 3 tháng))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<RoomPricingDto>> GetAll()
        {
            return await _pricingService.GetAllAsync();
        }
    }
}

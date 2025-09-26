using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Amenities;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// AmenityController
    /// </summary>
    [Route("api/amenities")]
    [ApiController]
    [Authorize]
    public class AmenityController : BaseController
    {
        private readonly IAmenityService _amenityService;
        /// <summary>
        /// AmenityController init
        /// </summary>
        /// <param name="amenityService"></param>
        /// <param name="httpContextAccessor"></param>
        public AmenityController(IAmenityService amenityService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _amenityService = amenityService;
        }

        /// <summary>
        /// Tạo tiện nghi
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<AmenityDto> Create([FromBody] CreateUpdateAmenityDto input)
        {
            return await _amenityService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin tiện nghi theo ID tiện nghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("{id}")]
        public async Task<AmenityDto> GetById(Guid id)
        {
            return await _amenityService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách tiện nghi kết hợp filter(status, startDate, endDate) và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("search")]
        public async Task<PagedResultDto<AmenityDto>> GetWithFilter([FromQuery] AmenityFilterDto request)
        {
            return await _amenityService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin tiện nghi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<AmenityDto> Update(Guid id, [FromBody] CreateUpdateAmenityDto input)
        {
            return await _amenityService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách tiện nghi với phòng liên quan
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("rooms/{id}")]
        public async Task<List<RoomAmenityDto>> GetRoomAmenities(Guid id)
        {
            return await _amenityService.GetRoomAmenitiesByAmenityAsync(id);
        }
        /// <summary>
        /// Xóa tiện nghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _amenityService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ tiện nghi 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<AmenityDto>> GetAll()
        {
            return await _amenityService.GetAllAsync();
        }

        /// <summary>
        /// Lấy danh sách tiện nghi theo loại
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("search-by-type")]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<AmenityDto>> GetByType([FromQuery] AmenityTypes type)
        {
            return await _amenityService.GetByTypeAsync(type);
        }
    }
}

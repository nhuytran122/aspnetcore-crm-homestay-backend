using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.ServiceItems;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// ServiceItemController
    /// </summary>
    [Route("api/service-items")]
    [ApiController]
    [Authorize]
    public class ServiceItemController : BaseController
    {
        private readonly IServiceItemService _itemService;
        /// <summary>
        /// ServiceItemController init
        /// </summary>
        /// <param name="itemService"></param>
        /// <param name="httpContextAccessor"></param>
        public ServiceItemController(IServiceItemService itemService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Tạo thiết bị thiết bị dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<ServiceItemDto> Create([FromBody] CreateServiceItemDto input)
        {
            return await _itemService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin thiết bị dịch vụ theo ID thiết bị dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ServiceItemDto> GetById(Guid id)
        {
            return await _itemService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách thiết bị dịch vụ kết hợp filter và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<PagedResultDto<ServiceItemDto>> GetWithFilter([FromQuery] ServiceItemFilterDto request)
        {
            return await _itemService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin thiết bị dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<ServiceItemDto> Update(Guid id, [FromBody] UpdateServiceItemDto input)
        {
            return await _itemService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa thiết bị dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _itemService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ thiết bị dịch vụ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<ServiceItemDto>> GetAll()
        {
            return await _itemService.GetAllAsync();
        }

        /// <summary>
        /// Lấy danh sách ServiceItem theo serviceId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpGet("by-service/{serviceId}")]
        public async Task<List<ServiceItemDto>> GetByHomestayServiceId(Guid serviceId)
        {
            return await _itemService.GetByHomestayServiceIdAsync(serviceId);
        }

    }
}

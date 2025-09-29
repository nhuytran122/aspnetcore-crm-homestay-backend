using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.HomestayServices;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// HomestayServiceController
    /// </summary>
    [Route("api/services")]
    [ApiController]
    [Authorize]
    public class HomestayServiceController : BaseController
    {
        private readonly IHomestayServiceService _homestayServiceService;
        /// <summary>
        /// HomestayServiceController init
        /// </summary>
        /// <param name="homestayServiceService"></param>
        /// <param name="httpContextAccessor"></param>
        public HomestayServiceController(IHomestayServiceService homestayServiceService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _homestayServiceService = homestayServiceService;
        }

        /// <summary>
        /// Tạo dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<HomestayServiceDto> Create([FromBody] CreateUpdateHomestayServiceDto input)
        {
            return await _homestayServiceService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin dịch vụ theo ID dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<HomestayServiceDetailDto> GetById(Guid id)
        {
            return await _homestayServiceService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách dịch vụ kết hợp filter và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<PagedResultDto<HomestayServiceDto>> GetWithFilter([FromQuery] HomestayServiceFilterDto request)
        {
            return await _homestayServiceService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<HomestayServiceDto> Update(Guid id, [FromBody] CreateUpdateHomestayServiceDto input)
        {
            return await _homestayServiceService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _homestayServiceService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ dịch vụ ( hoạt động + không + xóa mềm(chỉ lấy những chi nhánh xóa trong vòng 3 tháng))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<HomestayServiceDto>> GetAll()
        {
            return await _homestayServiceService.GetAllAsync();
        }
    }
}

using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Branches;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// BranchController
    /// </summary>
    [Route("api/branches")]
    [ApiController]
    [Authorize]
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;
        /// <summary>
        /// BranchController init
        /// </summary>
        /// <param name="branchService"></param>
        /// <param name="httpContextAccessor"></param>
        public BranchController(IBranchService branchService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _branchService = branchService;
        }

        /// <summary>
        /// Tạo chi nhánh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<BranchDto> Create([FromForm] CreateUpdateBranchDto input)
        {
            return await _branchService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi nhánh theo ID chi nhánh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("{id}")]
        public async Task<BranchDto> GetById(Guid id)
        {
            return await _branchService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách chi nhánh kết hợp filter(tên/sdt, status, startDate, endDate) và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("search")]
        public async Task<PagedResultDto<BranchDto>> GetWithFilter([FromQuery] BranchFilterDto request)
        {
            return await _branchService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin chi nhánh
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<BranchDto> Update(Guid id, [FromForm] CreateUpdateBranchDto input)
        {
            return await _branchService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách chi nhánh đang hoạt động
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("active")]
        public async Task<List<BranchDto>> GetAllActive()
        {
            return await _branchService.GetAllActiveAsync();
        }

        /// <summary>
        /// Lấy toàn bộ trạng thái của chi nhánh
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet]
        [Route("branch-statuses")]
        public Dictionary<string, int> GetAllBranchStatuses()
        {
            var result = new Dictionary<string, int>();
            var values = Enum.GetValues(typeof(BranchStatuses));
            foreach (var value in values)
            {
                result.Add(((BranchStatuses)value).GetDescription(), (int)value);
            }
            return result;
        }

        /// <summary>
        /// Xóa chi nhánh
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _branchService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ chi nhánh ( hoạt động + không + xóa mềm(chỉ lấy những chi nhánh xóa trong vòng 3 tháng))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<BranchDto>> GetAll()
        {
            return await _branchService.GetAllAsync();
        }
    }
}

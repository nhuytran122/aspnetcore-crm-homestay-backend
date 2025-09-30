using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Rules;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// RuleController
    /// </summary>
    [Route("api/rules")]
    [ApiController]
    [Authorize]
    public class RuleController : BaseController
    {
        private readonly IRuleService _ruleService;
        /// <summary>
        /// RuleController init
        /// </summary>
        /// <param name="ruleService"></param>
        /// <param name="httpContextAccessor"></param>
        public RuleController(IRuleService ruleService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _ruleService = ruleService;
        }

        /// <summary>
        /// Tạo Rule
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<RuleDto> Create([FromBody] CreateUpdateRuleDto input)
        {
            return await _ruleService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy danh sách Rule kết hợp filter(status, startDate, endDate) và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("search")]
        public async Task<PagedResultDto<RuleDto>> GetWithFilter([FromQuery] RuleFilterDto request)
        {
            return await _ruleService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin Rule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<RuleDto> Update(Guid id, [FromBody] CreateUpdateRuleDto input)
        {
            return await _ruleService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa rule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _ruleService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ Rule (đã active) 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<RuleDto>> GetAll()
        {
            return await _ruleService.GetAllAsync();
        }
    }
}

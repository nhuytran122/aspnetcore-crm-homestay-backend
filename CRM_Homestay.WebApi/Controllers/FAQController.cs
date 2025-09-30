using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.FAQs;
using CRM_Homestay.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Controllers
{
    /// <summary>
    /// FAQController
    /// </summary>
    [Route("api/faqs")]
    [ApiController]
    [Authorize]
    public class FAQController : BaseController
    {
        private readonly IFAQService _faqService;
        /// <summary>
        /// FAQController init
        /// </summary>
        /// <param name="faqService"></param>
        /// <param name="httpContextAccessor"></param>
        public FAQController(IFAQService faqService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _faqService = faqService;
        }

        /// <summary>
        /// Tạo FAQ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPost]
        public async Task<FAQDto> Create([FromBody] CreateUpdateFAQDto input)
        {
            return await _faqService.CreateAsync(input);
        }

        /// <summary>
        /// Lấy thông tin FAQ theo ID FAQ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("{id}")]
        public async Task<FAQDto> GetById(Guid id)
        {
            return await _faqService.GetByIdAsync(id);
        }

        /// <summary>
        /// Lấy danh sách FAQ kết hợp filter(status, startDate, endDate) và phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ALL)]
        [HttpGet("search")]
        public async Task<PagedResultDto<FAQDto>> GetWithFilter([FromQuery] FAQFilterDto request)
        {
            return await _faqService.GetPagingWithFilterAsync(request);
        }

        /// <summary>
        /// Cập nhật thông tin FAQ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpPut("{id}")]
        public async Task<FAQDto> Update(Guid id, [FromBody] CreateUpdateFAQDto input)
        {
            return await _faqService.UpdateAsync(id, input);
        }

        /// <summary>
        /// Xóa faq
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleCodes.ADMIN_AND_TECHNICAL)]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _faqService.DeleteAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ FAQ (đã active) 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleCodes.ALL)]
        public async Task<List<FAQDto>> GetAll()
        {
            return await _faqService.GetAllAsync();
        }
    }
}

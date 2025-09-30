using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Entity.FAQs;
using CRM_Homestay.Contract.FAQs;

namespace CRM_Homestay.Service.FAQs
{
    public class FAQService : BaseService, IFAQService, ITransientDependency
    {
        public FAQService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

        public async Task<FAQDto> CreateAsync(CreateUpdateFAQDto input)
        {
            HandleInput(input);
            if (await _unitOfWork.GenericRepository<FAQ>().AnyAsync(x => x.Question!.ToLower() == input.Question!.ToLower()))
            {
                throw new GlobalException(code: FAQErrorCode.AlreadyExists,
                        message: L[FAQErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var faq = ObjectMapper.Map<CreateUpdateFAQDto, FAQ>(input);

            faq.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.Question} {input.Answer}");

            await _unitOfWork.GenericRepository<FAQ>().AddAsync(faq);
            await _unitOfWork.SaveChangeAsync();
            return ObjectMapper.Map<FAQ, FAQDto>(faq);
        }

        public async Task<FAQDto> GetByIdAsync(Guid id)
        {
            var faq = await _unitOfWork.GenericRepository<FAQ>().GetAsync(x => x.Id == id);
            if (faq == null)
            {
                throw new GlobalException(code: FAQErrorCode.NotFound,
                        message: L[FAQErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            return ObjectMapper.Map<FAQ, FAQDto>(faq);
        }

        public async Task<FAQDto> UpdateAsync(Guid id, CreateUpdateFAQDto input)
        {
            var faq = await _unitOfWork.GenericRepository<FAQ>().GetAsync(x => x.Id == id);
            if (faq == null)
            {
                throw new GlobalException(code: FAQErrorCode.NotFound,
                        message: L[FAQErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            var result = await _unitOfWork.GenericRepository<FAQ>().GetListAsync(x => x.Id != id && x.Question.ToLower() == input.Question!.ToLower());
            if (result.Count > 0)
            {
                throw new GlobalException(code: FAQErrorCode.AlreadyExists,
                        message: L[FAQErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }
            HandleInput(input);
            var newFAQInfo = ObjectMapper.Map(input, faq);
            newFAQInfo.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.Question} {input.Answer}");
            _unitOfWork.GenericRepository<FAQ>().Update(newFAQInfo);
            await _unitOfWork.SaveChangeAsync();

            return ObjectMapper.Map<FAQ, FAQDto>(newFAQInfo);
        }

        private void HandleInput(CreateUpdateFAQDto input)
        {
            input.Question = input.Question!.Trim();
            input.Answer = input.Answer!.Trim();
        }

        public async Task<PagedResultDto<FAQDto>> GetPagingWithFilterAsync(FAQFilterDto input)
        {
            var searchTerm = !string.IsNullOrEmpty(input.Text) ? $" {NormalizeString.ConvertNormalizeString(input.Text)} " : string.Empty;
            var query = _unitOfWork.GenericRepository<FAQ>()
                                                        .GetQueryable()
                                                        .OrderByDescending(x => x.CreationTime)
                                                        .WhereIf(!string.IsNullOrEmpty(input.Text),
                                                        x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm))
                                                        .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                                                        .WhereIf(input.StartDate != null, x => x.CreationTime.AddHours(7).Date >= input.StartDate!.Value.Date)
                                                        .WhereIf(input.EndDate != null, x => x.CreationTime.AddHours(7).Date <= input.EndDate!.Value.Date)
                                                        .Select(x => new FAQDto()
                                                        {
                                                            Id = x.Id,
                                                            Question = x.Question,
                                                            Answer = x.Answer,
                                                            CreationTime = x.CreationTime,
                                                        });

            var data = await query.GetPaged(input.CurrentPage, input.PageSize);
            return ObjectMapper.Map<PagedResult<FAQDto>, PagedResultDto<FAQDto>>(data);
        }

        public async Task DeleteAsync(Guid id)
        {
            var faq = await _unitOfWork.GenericRepository<FAQ>().GetAsync(x => x.Id == id);
            if (faq == null)
            {
                throw new GlobalException(code: FAQErrorCode.NotFound,
                        message: L[FAQErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            _unitOfWork.GenericRepository<FAQ>().Remove(faq);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<FAQDto>> GetAllAsync()
        {
            var faqs = await _unitOfWork.GenericRepository<FAQ>()
                .GetQueryable()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
            return ObjectMapper.Map<List<FAQ>, List<FAQDto>>(faqs);
        }
    }
}

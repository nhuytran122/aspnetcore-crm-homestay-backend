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
using CRM_Homestay.Entity.Rules;
using CRM_Homestay.Contract.Rules;

namespace CRM_Homestay.Service.Rules
{
    public class RuleService : BaseService, IRuleService, ITransientDependency
    {
        public RuleService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

        public async Task<RuleDto> CreateAsync(CreateUpdateRuleDto input)
        {
            HandleInput(input);
            if (await _unitOfWork.GenericRepository<Rule>().AnyAsync(x => x.Title!.ToLower() == input.Title!.ToLower()))
            {
                throw new GlobalException(code: RuleErrorCode.AlreadyExists,
                        message: L[RuleErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var rule = ObjectMapper.Map<CreateUpdateRuleDto, Rule>(input);

            rule.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.Title} {input.Description}");

            await _unitOfWork.GenericRepository<Rule>().AddAsync(rule);
            await _unitOfWork.SaveChangeAsync();
            return ObjectMapper.Map<Rule, RuleDto>(rule);
        }

        public async Task<RuleDto> UpdateAsync(Guid id, CreateUpdateRuleDto input)
        {
            var rule = await _unitOfWork.GenericRepository<Rule>().GetAsync(x => x.Id == id);
            if (rule == null)
            {
                throw new GlobalException(code: RuleErrorCode.NotFound,
                        message: L[RuleErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            var result = await _unitOfWork.GenericRepository<Rule>().GetListAsync(x => x.Id != id && x.Title.ToLower() == input.Title!.ToLower());
            if (result.Count > 0)
            {
                throw new GlobalException(code: RuleErrorCode.AlreadyExists,
                        message: L[RuleErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }
            HandleInput(input);
            var newRuleInfo = ObjectMapper.Map(input, rule);
            newRuleInfo.NormalizeFullInfo = NormalizeString.ConvertNormalizeString($"{input.Title} {input.Description}");
            _unitOfWork.GenericRepository<Rule>().Update(newRuleInfo);
            await _unitOfWork.SaveChangeAsync();

            return ObjectMapper.Map<Rule, RuleDto>(newRuleInfo);
        }

        private void HandleInput(CreateUpdateRuleDto input)
        {
            input.Title = input.Title!.Trim();
            input.Description = input.Description!.Trim();
        }

        public async Task<PagedResultDto<RuleDto>> GetPagingWithFilterAsync(RuleFilterDto input)
        {
            var searchTerm = !string.IsNullOrEmpty(input.Text) ? $" {NormalizeString.ConvertNormalizeString(input.Text)} " : string.Empty;
            var query = _unitOfWork.GenericRepository<Rule>()
                                                        .GetQueryable()
                                                        .OrderByDescending(x => x.CreationTime)
                                                        .WhereIf(!string.IsNullOrEmpty(input.Text),
                                                        x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm))
                                                        .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                                                        .WhereIf(input.StartDate != null, x => x.CreationTime.AddHours(7).Date >= input.StartDate!.Value.Date)
                                                        .WhereIf(input.EndDate != null, x => x.CreationTime.AddHours(7).Date <= input.EndDate!.Value.Date)
                                                        .Select(x => new RuleDto()
                                                        {
                                                            Id = x.Id,
                                                            Title = x.Title,
                                                            Description = x.Description,
                                                            CreationTime = x.CreationTime,
                                                        });

            var data = await query.GetPaged(input.CurrentPage, input.PageSize);
            return ObjectMapper.Map<PagedResult<RuleDto>, PagedResultDto<RuleDto>>(data);
        }

        public async Task DeleteAsync(Guid id)
        {
            var rule = await _unitOfWork.GenericRepository<Rule>().GetAsync(x => x.Id == id);
            if (rule == null)
            {
                throw new GlobalException(code: RuleErrorCode.NotFound,
                        message: L[RuleErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            _unitOfWork.GenericRepository<Rule>().Remove(rule);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<RuleDto>> GetAllAsync()
        {
            var rules = await _unitOfWork.GenericRepository<Rule>()
                .GetQueryable()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
            return ObjectMapper.Map<List<Rule>, List<RuleDto>>(rules);
        }
    }
}

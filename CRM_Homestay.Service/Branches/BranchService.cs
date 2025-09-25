using AutoMapper;
using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Contract.Locations;
using CRM_Homestay.Contract.Branches;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Helper;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BranchInventories;
using CRM_Homestay.Entity.Branches;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;
using CRM_Homestay.Contract.Uploads;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Medias;

namespace CRM_Homestay.Service.Branches
{
    public class BranchService : BaseService, IBranchService, ITransientDependency
    {
        private readonly ILocationServiceShared _locationService;
        private readonly IUploadService _uploadService;
        public BranchService(ILocationServiceShared locationService, IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l, IUploadService uploadService) : base(unitOfWork, mapper, l)
        {
            _locationService = locationService;
            _uploadService = uploadService;
        }

        public async Task<BranchDto> CreateAsync(CreateUpdateBranchDto input)
        {
            HandleInput(input);
            if (await _unitOfWork.GenericRepository<Branch>().AnyAsync(x => x.Name!.ToLower() == input.Name!.ToLower()))
            {
                throw new GlobalException(code: BranchErrorCode.AlreadyExists,
                        message: L[BranchErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }

            var branch = ObjectMapper.Map<CreateUpdateBranchDto, Branch>(input);

            branch.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);

            var inputAddress = input.Address;
            var location = await _locationService.GetLocations(new LocationRequestDto()
            {
                ProvinceId = inputAddress.ProvinceId,
                DistrictId = inputAddress.DistrictId,
                WardId = inputAddress.WardId,
            });
            var branchAddress = branch.Address;

            branchAddress!.JoinedName = $"{input.Address.Street}, {location.Ward}, {location.District}, {location.Province}";
            branchAddress.WardName = location.Ward;
            branchAddress.DistrictName = location.District;
            branchAddress.ProvinceName = location.Province;
            branch.NormalizeAddress = NormalizeString.ConvertNormalizeString(branchAddress.JoinedName.Replace(",", ""));
            if (input.Image != null)
            {
                BaseMedia image = await _uploadService.UploadImg(input.Image, "Media:Branches");
                await _unitOfWork.GenericRepository<BaseMedia>().AddAsync(image);
                branch.Media = image;
            }
            if (input.IsMainBranch)
            {
                var existingMainBranch = await _unitOfWork.GenericRepository<Branch>()
                    .GetQueryable()
                    .Where(x => x.IsMainBranch)
                    .FirstOrDefaultAsync();

                if (existingMainBranch != null)
                {
                    existingMainBranch.IsMainBranch = false;
                    _unitOfWork.GenericRepository<Branch>().Update(existingMainBranch);
                }
            }
            await _unitOfWork.GenericRepository<Branch>().AddAsync(branch);
            await _unitOfWork.SaveChangeAsync();
            var result = ObjectMapper.Map<Branch, BranchDto>(branch);
            await GetLocationInfo(result);
            string rootUrl = _uploadService.GetRootUrl();
            result.MediaUrl = branch.Media != null ? rootUrl + branch.Media.FilePath : null;
            return result;
        }

        public async Task<BranchDto> GetByIdAsync(Guid id)
        {
            var branch = await _unitOfWork.GenericRepository<Branch>()
                                                        .GetQueryable()
                                                        .AsNoTracking()
                                                        .Include(x => x.Media)
                                                        .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (branch == null)
            {
                throw new GlobalException(code: BranchErrorCode.NotFound,
                        message: L[BranchErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            string rootUrl = _uploadService.GetRootUrl();
            var branchDto = ObjectMapper.Map<Branch, BranchDto>(branch);
            await GetLocationInfo(branchDto);
            branchDto.MediaUrl = branch.Media != null ? rootUrl + branch.Media.FilePath : null;
            return branchDto;
        }

        public async Task<BranchDto> UpdateAsync(Guid id, CreateUpdateBranchDto input)
        {
            var branch = await _unitOfWork.GenericRepository<Branch>()
                                                        .GetQueryable()
                                                        .AsNoTracking()
                                                        .Include(x => x.Media)
                                                        .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (branch == null)
            {
                throw new GlobalException(code: BranchErrorCode.NotFound,
                        message: L[BranchErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            var result = await _unitOfWork.GenericRepository<Branch>().GetListAsync(x => x.Id != id && x.Name.ToLower() == input.Name.ToLower());
            if (result.Count > 0)
            {
                throw new GlobalException(code: BranchErrorCode.AlreadyExists,
                        message: L[BranchErrorCode.AlreadyExists],
                        statusCode: HttpStatusCode.BadRequest);
            }
            if (input.IsMainBranch && !branch.IsMainBranch)
            {
                var existingMainBranch = await _unitOfWork.GenericRepository<Branch>()
                    .GetQueryable()
                    .Where(x => x.IsMainBranch && x.Id != id)
                    .FirstOrDefaultAsync();

                if (existingMainBranch != null)
                {
                    existingMainBranch.IsMainBranch = false;
                    _unitOfWork.GenericRepository<Branch>().Update(existingMainBranch);
                }
            }
            else if (!input.IsMainBranch && branch.IsMainBranch) // Nếu bỏ main branch, cần đảm bảo có main branch khác
            {
                // var otherBranches = await _unitOfWork.GenericRepository<Branch>()
                //     .GetQueryable()
                //     .Where(x => x.Id != id && !x.DeletedAt.HasValue)
                //     .ToListAsync();

                // if (!otherBranches.Any(x => x.IsMainBranch))
                // {
                throw new GlobalException(code: BranchErrorCode.MustHaveMainBranch,
                    message: L[BranchErrorCode.MustHaveMainBranch],
                    statusCode: HttpStatusCode.BadRequest);
                // }
            }
            var newBranchInfo = ObjectMapper.Map(input, branch);
            newBranchInfo.NormalizeFullInfo = NormalizeString.ConvertNormalizeString(input.Name!);

            var inputAddress = input.Address;
            var location = await _locationService.GetLocations(new LocationRequestDto()
            {
                ProvinceId = inputAddress.ProvinceId,
                DistrictId = inputAddress.DistrictId,
                WardId = inputAddress.WardId,
            });

            var branchAddress = newBranchInfo.Address;

            branchAddress!.JoinedName = $"{input.Address.Street}, {location.Ward}, {location.District}, {location.Province}";
            branchAddress.WardName = location.Ward;
            branchAddress.DistrictName = location.District;
            branchAddress.ProvinceName = location.Province;
            newBranchInfo.NormalizeAddress = NormalizeString.ConvertNormalizeString(branchAddress.JoinedName.Replace(",", ""));

            if (input.Image != null)
            {
                if (branch.Media != null)
                {
                    _uploadService.DeleteImage(branch.Media.FilePath);
                    _unitOfWork.GenericRepository<BaseMedia>().Remove(branch.Media);
                }

                BaseMedia image = await _uploadService.UploadImg(input.Image, "Media:Branches");
                await _unitOfWork.GenericRepository<BaseMedia>().AddAsync(image);
                newBranchInfo.Media = image;
            }
            else
            {
                newBranchInfo.Media = branch.Media;
            }
            _unitOfWork.GenericRepository<Branch>().Update(newBranchInfo);
            await _unitOfWork.SaveChangeAsync();

            var updatedBranch = ObjectMapper.Map<Branch, BranchDto>(newBranchInfo);
            string rootUrl = _uploadService.GetRootUrl();
            await GetLocationInfo(updatedBranch);
            updatedBranch.MediaUrl = newBranchInfo.Media != null ? rootUrl + newBranchInfo.Media.FilePath : null;
            return updatedBranch;
        }

        private void HandleInput(CreateUpdateBranchDto input)
        {
            input.Name = input.Name.Trim();
            input.Name = Regex.Replace(input.Name, @"\s+", " ");
            input.Address.Street = input.Address.Street!.Trim();
            input.PhoneNumber = input.PhoneNumber.Trim();
            input.GatePassword = input.GatePassword.Trim();
        }

        public async Task<PagedResultDto<BranchDto>> GetPagingWithFilterAsync(BranchFilterDto input)
        {
            var searchTerm = !string.IsNullOrEmpty(input.Text) ? $" {NormalizeString.ConvertNormalizeString(input.Text)} " : string.Empty;
            string rootUrl = _uploadService.GetRootUrl();
            var query = _unitOfWork.GenericRepository<Branch>()
                                                        .GetQueryable()
                                                        .Include(x => x.Media)
                                                        .OrderByDescending(x => x.CreationTime)
                                                        .Where(x => !x.DeletedAt.HasValue)
                                                        .WhereIf(!string.IsNullOrEmpty(input.Text),
                                                        x => (" " + x.NormalizeFullInfo + " ").Contains(searchTerm) ||
                                                        (" " + x.PhoneNumber + " ").Contains(searchTerm))
                                                        .WhereIf(input.Status != null, x => x.Status == input.Status)
                                                        .WhereIf(input.StartDate != null, x => x.CreationTime.AddHours(7).Date >= input.StartDate!.Value.Date)
                                                        .WhereIf(input.EndDate != null, x => x.CreationTime.AddHours(7).Date <= input.EndDate!.Value.Date)
                                                        .Select(x => new BranchDto()
                                                        {
                                                            Id = x.Id,
                                                            Name = x.Name,
                                                            Address = x.Address!,
                                                            CreationTime = x.CreationTime,
                                                            PhoneNumber = x.PhoneNumber,
                                                            Status = x.Status,
                                                            IsMainBranch = x.IsMainBranch,
                                                            GatePassword = x.GatePassword,
                                                            MediaUrl = x.Media != null ? rootUrl + x.Media.FilePath : null,
                                                        });

            var data = await query.GetPaged(input.PageIndex, input.PageSize);
            var result = ObjectMapper.Map<PagedResult<BranchDto>, PagedResultDto<BranchDto>>(data);

            foreach (var item in result.Items)
            {
                await GetLocationInfo(item);
            }
            return result;
        }

        public async Task<List<BranchDto>> GetAllActiveAsync()
        {
            string rootUrl = _uploadService.GetRootUrl();
            var result = await _unitOfWork.GenericRepository<Branch>()
                .GetQueryable()
                .Where(x => x.Status == BranchStatuses.Active && !x.DeletedAt.HasValue)
                .Select(x => new BranchDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address!,
                    CreationTime = x.CreationTime,
                    PhoneNumber = x.PhoneNumber,
                    Status = x.Status,
                    IsMainBranch = x.IsMainBranch,
                    GatePassword = x.GatePassword,
                    MediaUrl = x.Media != null ? rootUrl + x.Media.FilePath : null,
                })
                .ToListAsync();
            if (result.Any())
            {
                var tasks = result.Select(branch => GetLocationInfo(branch));
                await Task.WhenAll(tasks);
            }
            return result;
        }

        private async Task GetLocationInfo(BranchDto branch)
        {
            branch.LocationName = new LocationDto();
            var locationRequest = new LocationRequestDto(branch.Address.ProvinceId, branch.Address.DistrictId, branch.Address.WardId);
            var addressInfo = await _locationService.GetLocations(locationRequest);
            branch.LocationName.Province = addressInfo.Province;
            branch.LocationName.District = addressInfo.District;
            branch.LocationName.Ward = addressInfo.Ward;
        }

        public async Task DeleteAsync(Guid id)
        {
            var branch = await _unitOfWork.GenericRepository<Branch>().GetAsync(x => x.Id == id && !x.DeletedAt.HasValue);
            if (branch == null)
            {
                throw new GlobalException(code: BranchErrorCode.NotFound,
                        message: L[BranchErrorCode.NotFound],
                        statusCode: HttpStatusCode.NotFound);
            }
            if (branch.IsMainBranch)
            {
                throw new GlobalException(code: BranchErrorCode.IsMainBranch,
                        message: L[BranchErrorCode.IsMainBranch],
                        statusCode: HttpStatusCode.BadRequest);
            }
            if (await HasInventory(id))
            {
                throw new GlobalException(code: BranchErrorCode.NotAllowedDelete,
                        message: L[BranchErrorCode.NotAllowedDelete],
                        statusCode: HttpStatusCode.BadRequest);
            }
            branch.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            branch.Status = BranchStatuses.Inactive;
            _unitOfWork.GenericRepository<Branch>().Update(branch);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<BranchDto>> GetAllAsync()
        {
            var threeMonthsAgo = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(-3), DateTimeKind.Unspecified);
            string rootUrl = _uploadService.GetRootUrl();

            var branches = await _unitOfWork.GenericRepository<Branch>()
                .GetQueryable()
                .Include(x => x.Media)
                .Where(x => !x.DeletedAt.HasValue || (x.DeletedAt.HasValue && x.DeletedAt.Value.Date >= threeMonthsAgo.Date))
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new BranchDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address!,
                    CreationTime = x.CreationTime,
                    PhoneNumber = x.PhoneNumber,
                    Status = x.Status,
                    IsMainBranch = x.IsMainBranch,
                    GatePassword = x.GatePassword,
                    MediaUrl = x.Media != null ? rootUrl + x.Media.FilePath : null,
                })
                .ToListAsync();
            // if (branches.Any())
            // {
            //     foreach (var branch in branches)
            //     {
            //         await GetLocationInfo(branch);
            //     }
            // }
            return branches;
        }

        private async Task<bool> HasInventory(Guid id)
        {
            if (await _unitOfWork.GenericRepository<BranchInventory>().AnyAsync(x => x.BranchId == id && x.QuantityOnHand > 0))
                return true;
            return false;
        }
    }
}

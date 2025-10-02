
using AutoMapper;
using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Contract.SystemSettings;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.SystemSettings;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM_Homestay.Service.SystemSettings
{
    public class SystemSettingService : BaseService, ISystemSettingService
    {
        public SystemSettingService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

        public async Task<OvernightPeriodDto> GetOvernightPeriodAsync()
        {
            var configRepo = _unitOfWork.GenericRepository<SystemSetting>();

            var startConfig = await configRepo.GetQueryable().AsNoTracking()
                .FirstOrDefaultAsync(x => x.SystemName == ConfigKey.RoomPricing
                                       && x.ConfigKey == ConfigKey.OvernightStartTime);
            if (startConfig == null)
                throw new GlobalException(
                    code: SystemSettingErrorCode.NotFound,
                    message: L[SystemSettingErrorCode.NotFound, L[BaseErrorCode.OvernightStartTime]],
                    statusCode: HttpStatusCode.BadRequest);

            var endConfig = await configRepo.GetQueryable().AsNoTracking()
                .FirstOrDefaultAsync(x => x.SystemName == ConfigKey.RoomPricing
                                       && x.ConfigKey == ConfigKey.OvernightEndTime);
            if (endConfig == null)
                throw new GlobalException(
                    code: SystemSettingErrorCode.NotFound,
                    message: L[SystemSettingErrorCode.NotFound, L[BaseErrorCode.OvernightEndTime]],
                    statusCode: HttpStatusCode.BadRequest);

            return new OvernightPeriodDto
            {
                OvernightStart = TimeSpan.Parse(startConfig.ConfigValue),
                OvernightEnd = TimeSpan.Parse(endConfig.ConfigValue)
            };
        }

        public async Task<int> GetCleaningMinutesAsync()
        {
            var configSettingRepo = _unitOfWork.GenericRepository<SystemSetting>();

            var setting = await configSettingRepo.GetQueryable()
                .AsNoTracking()
                .Where(x => x.SystemName == ConfigKey.RoomUsage && x.ConfigKey == ConfigKey.CleaningMinutes)
                .FirstOrDefaultAsync();

            if (setting == null)
                throw new GlobalException(code: SystemSettingErrorCode.NotFound,
                        message: L[SystemSettingErrorCode.NotFound],
                        statusCode: HttpStatusCode.BadRequest);

            return int.Parse(setting.ConfigValue);
        }

    }
}

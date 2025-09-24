
using AutoMapper;
using CRM_Homestay.Contract.SystemSettings;
using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.SystemSettings;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace CRM_Homestay.Service.SystemSettings
{
    public class SystemSettingService : BaseService, ISystemSettingService
    {
        public SystemSettingService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l) : base(unitOfWork, mapper, l)
        {
        }

    }
}

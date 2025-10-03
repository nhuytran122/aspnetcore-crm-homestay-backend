using CRM_Homestay.Localization;
using Microsoft.Extensions.Configuration;

namespace CRM_Homestay.Service.OtpCodes.Providers
{
    public abstract class BaseOtpProvider
    {
        protected ILocalizer L { get; }

        protected IConfiguration _configuration { get; }
        public BaseOtpProvider(ILocalizer l, IConfiguration configuration)
        {
            L = l;
            _configuration = configuration;
        }
    }
}
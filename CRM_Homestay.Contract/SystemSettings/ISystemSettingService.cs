
using CRM_Homestay.Contract.RoomPricings;

namespace CRM_Homestay.Contract.SystemSettings
{
    public interface ISystemSettingService
    {
        Task<OvernightPeriodDto> GetOvernightPeriodAsync();
        Task<int> GetCleaningMinutesAsync();
    }
}

using CRM_Homestay.Core.Consts;
using CRM_Homestay.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRM_Homestay.Core.Helpers
{
    public static class DateRangeFilter
    {
        public static IQueryable<T> ApplyDateRangeFilter<T>(this IQueryable<T> query, DateTime? startDate, DateTime? endDate) where T : class
        {
            if (startDate != null && endDate == null)
            {
                var start = startDate.Value.Date;
                query = query.Where(x => EF.Property<DateTime>(x, ConstAdvanceSearch.CreationTime).Date >= start);
            }

            if (startDate != null && endDate != null)
            {
                var start = startDate.Value.Date;
                var end = endDate.Value.Date;
                query = query.Where(x => EF.Property<DateTime>(x, ConstAdvanceSearch.CreationTime).Date >= start && EF.Property<DateTime>(x, ConstAdvanceSearch.CreationTime).Date <= end);
            }

            if (startDate == null && endDate != null)
            {
                var end = endDate.Value.Date;
                query = query.Where(x => EF.Property<DateTime>(x, ConstAdvanceSearch.CreationTime).Date <= end);
            }

            return query;
        }
    }
}

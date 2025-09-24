using CRM_Homestay.Entity.BookingServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Homestay.Database.Configurations
{
    public class BookingServiceConfiguration : IEntityTypeConfiguration<BookingService>
    {
        public void Configure(EntityTypeBuilder<BookingService> builder)
        {
            builder.HasOne(bs => bs.AssignedStaff)
            .WithMany(u => u.AssignedServices)
            .HasForeignKey(bs => bs.AssignedStaffId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(bs => bs.Creator)
            .WithMany(u => u.CreatedServices)
            .HasForeignKey(bs => bs.CreatorId)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

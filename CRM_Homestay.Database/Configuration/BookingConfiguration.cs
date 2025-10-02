using CRM_Homestay.Entity.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Homestay.Database.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BookingRooms)
                .WithOne(br => br.Booking)
                .HasForeignKey(br => br.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.BookingServices)
                .WithOne(br => br.Booking)
                .HasForeignKey(br => br.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.BookingParent)
                .WithMany(bp => bp.SubBookings)
                .HasForeignKey(b => b.BookingParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

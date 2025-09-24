using CRM_Homestay.Core.Enums;
using CRM_Homestay.Entity.Bases;
using CRM_Homestay.Entity.BookingPaymentDetails;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.Users;

namespace CRM_Homestay.Entity.BookingPayments
{
    public class BookingPayment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public PaymentStatuses PaymentStatus { get; set; } = PaymentStatuses.Pending;
        public DateTime PaymentDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        public PaymentMethods PaymentMethod { get; set; }
        public decimal Amount { get; set; } = 0;
        public bool? IsReturnedCash { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? ExternalTransactionId { get; set; }
        public Guid? StaffId { get; set; }

        public Booking? Booking { get; set; }
        public List<BookingPaymentDetail>? BookingPaymentDetails { get; set; }
        public User? Staff { get; set; }
    }

}

using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Models;
using CRM_Homestay.Entity.BookingPayments;
using CRM_Homestay.Entity.BookingServices;

namespace CRM_Homestay.Entity.BookingPaymentDetails
{
    public class BookingPaymentDetail : IAuditable
    {
        public Guid Id { get; set; }
        public Guid BookingPaymentId { get; set; }
        public Guid? BookingServiceId { get; set; }
        public Guid PaymentTypeId { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public PaymentPurposes Purpose { get; set; }

        public BookingPayment? BookingPayment { get; set; }
        public BookingService? BookingService { get; set; }
    }
}

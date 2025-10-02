using CRM_Homestay.Contract.Bookings;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator(Localizer L)
    {
        RuleFor(x => x.CustomerId)
            .NotNull()
            .WithMessage(L["Validator.IsRequired"]);

        RuleFor(x => x.CheckIn)
            .NotNull()
            .WithMessage(L["Validator.IsRequired"]);

        RuleFor(x => x.CheckOut)
            .NotNull()
            .WithMessage(L["Validator.IsRequired"]);

        // RuleFor(x => x.TotalGuests.ToString())
        //     .NotEmpty()
        //     .WithMessage(L["Validator.IsRequired"])
        //     .Matches(InputRegularExpression.Numeric)
        //     .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x)
            .Must(x =>
            {
                return x.CheckOut >= x.CheckIn;
            })
            .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);

        RuleForEach(x => x.BookingRooms)
            .NotEmpty()
                .WithMessage(L["Validator.IsRequired"])
            .SetValidator(new CreateBookingRoomValidator(L));
    }

    public class CreateBookingRoomValidator : AbstractValidator<CreateBookingRoomDto>
    {
        public CreateBookingRoomValidator(Localizer L)
        {
            RuleFor(x => x.RoomId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.GuestCounts.ToString())
                        .NotEmpty()
                        .WithMessage(L["Validator.IsRequired"])
                        .Matches(InputRegularExpression.Numeric)
                        .WithMessage(L["Validator.InValidFormat"]);
            RuleFor(x => x.GuestCounts)
                .Must(q => !q.HasValue || q >= 1)
                .WithMessage(L["Validator.GreaterThan", 1]);
        }
    }

    public class UpdateBookingValidator : AbstractValidator<UpdateBookingDto>
    {
        public UpdateBookingValidator(Localizer L)
        {
            RuleFor(x => x.CheckIn)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.CheckOut)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x)
                .Must(x =>
                {
                    return x.CheckOut >= x.CheckIn;
                })
                .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);

            RuleForEach(x => x.BookingRooms)
                .NotEmpty()
                    .WithMessage(L["Validator.IsRequired"])
                .SetValidator(new UpdateBookingRoomValidator(L));
        }

        public class UpdateBookingRoomValidator : AbstractValidator<UpdateBookingRoomDto>
        {
            public UpdateBookingRoomValidator(Localizer L)
            {
                RuleFor(x => x.RoomId)
                    .NotEmpty()
                    .WithMessage(L["Validator.IsRequired"]);

                RuleFor(x => x.GuestCounts.ToString())
                    .NotEmpty()
                    .WithMessage(L["Validator.IsRequired"])
                    .Matches(InputRegularExpression.Numeric)
                    .WithMessage(L["Validator.InValidFormat"]);
                RuleFor(x => x.GuestCounts)
                    .Must(q => !q.HasValue || q >= 1)
                    .WithMessage(L["Validator.GreaterThan", 1]);
            }
        }
    }
}

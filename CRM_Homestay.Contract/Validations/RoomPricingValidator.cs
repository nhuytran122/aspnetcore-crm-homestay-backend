using CRM_Homestay.Contract.RoomPricings;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class CreateRoomPricingDtoValidator : AbstractValidator<CreateRoomPricingDto>
    {
        public CreateRoomPricingDtoValidator(Localizer L)
        {
            RuleFor(x => x.RoomTypeId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

            RuleFor(x => x.BaseDuration)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(1).WithMessage(L["Validator.GreaterThan", 1]);

            RuleFor(x => x.BasePrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x.ExtraHourPrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x.OvernightPrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x.DailyPrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x)
                .Must(x => x.IsDefault || (x.StartAt != null && x.EndAt != null))
                .WithMessage(L[RoomPricingErrorCode.NonDefaultPricingDatesRequired]);

            RuleFor(x => x)
                .Must(x => !(x.StartAt.HasValue && x.EndAt.HasValue) || x.EndAt >= x.StartAt)
                .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);
        }
    }

    public class UpdateRoomPricingDtoValidator : AbstractValidator<UpdateRoomPricingDto>
    {
        public UpdateRoomPricingDtoValidator(Localizer L)
        {
            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

            RuleFor(x => x.BaseDuration)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(1).WithMessage(L["Validator.GreaterThan", 1]);

            RuleFor(x => x.BasePrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x.ExtraHourPrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x.OvernightPrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x.DailyPrice)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(0).WithMessage(L["Validator.MustBeGreaterThanOrEqualToZero"]);

            RuleFor(x => x)
                .Must(x => x.IsDefault || (x.StartAt != null && x.EndAt != null))
                .WithMessage(L[RoomPricingErrorCode.NonDefaultPricingDatesRequired]);

            RuleFor(x => x)
                .Must(x => !(x.StartAt.HasValue && x.EndAt.HasValue) || x.EndAt >= x.StartAt)
                .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);
        }
    }
}

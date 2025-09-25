using CRM_Homestay.Contract.RoomTypes;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class RoomTypeValidator : AbstractValidator<CreateUpdateRoomTypeDto>
    {
        public RoomTypeValidator(Localizer L)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(3)
                .WithMessage(L["Validator.TooShort", 3])
                .MaximumLength(200)
                .WithMessage(L["Validator.TooLong", 200])
                .Matches(InputRegularExpression.NotSpecialCharacter);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

            RuleFor(x => x.MaxGuests)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThan(1).WithMessage(L["Validator.GreaterThan", 1]);

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
        }
    }
}

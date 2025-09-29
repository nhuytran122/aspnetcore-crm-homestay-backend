using CRM_Homestay.Contract.RoomAmenities;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class CreateRoomAmenityDtoValidator : AbstractValidator<CreateRoomAmenityDto>
    {
        public CreateRoomAmenityDtoValidator(Localizer L)
        {
            RuleFor(x => x.RoomId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.AmenityId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Quantity.ToString())
                .Matches(InputRegularExpression.Numeric)
                .WithMessage(L["Validator.InValidFormat"]);
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1).WithMessage(L["Validator.GreaterThan", 1]);
        }
    }

    public class UpdateRoomAmenityDtoValidator : AbstractValidator<UpdateRoomAmenityDto>
    {
        public UpdateRoomAmenityDtoValidator(Localizer L)
        {
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .GreaterThanOrEqualTo(1).WithMessage(L["Validator.GreaterThan", 1]);
        }
    }
}

using CRM_Homestay.Contract.Amenities;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class AmenityValidator : AbstractValidator<CreateUpdateAmenityDto>
    {
        public AmenityValidator(Localizer L)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(3)
                .WithMessage(L["Validator.TooShort", 3])
                .MaximumLength(200)
                .WithMessage(L["Validator.TooLong", 200]);

            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(value => value.HasValue && Enum.IsDefined(typeof(AmenityTypes), value))
                .WithMessage(L["Validator.EnumInvalid"]);

        }
    }
}

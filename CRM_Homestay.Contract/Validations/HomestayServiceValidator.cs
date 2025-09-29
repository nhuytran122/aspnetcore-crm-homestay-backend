using CRM_Homestay.Contract.HomestayServices;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class CreateUpdateHomestayServiceDtoValidator : AbstractValidator<CreateUpdateHomestayServiceDto>
    {
        public CreateUpdateHomestayServiceDtoValidator(Localizer L)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Price.ToString())
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"])
                .Matches(InputRegularExpression.Numeric)
                .WithMessage(L["Validator.InValidFormat"]);

            RuleFor(x => x.IsPrepaid && x.HasInventory)
                .Must(x => x == true || x == false)
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);
            RuleForEach(x => x.ServiceItems!)
                .SetValidator(new CreateUpdateServiceItemInServiceValidator(L));
        }
    }
}

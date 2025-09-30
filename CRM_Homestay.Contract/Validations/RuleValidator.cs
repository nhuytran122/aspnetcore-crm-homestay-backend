using CRM_Homestay.Contract.Rules;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class RuleValidator : AbstractValidator<CreateUpdateRuleDto>
    {
        public RuleValidator(Localizer L)
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(3)
                .WithMessage(L["Validator.TooShort", 3])
                .MaximumLength(200)
                .WithMessage(L["Validator.TooLong", 200]);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(3)
                .WithMessage(L["Validator.TooShort", 3])
                .MaximumLength(200)
                .WithMessage(L["Validator.TooLong", 200]);

            RuleFor(x => x.IsActive)
                .Must(x => x == true || x == false)
                .WithMessage(L["Validator.InValidFormat"])
                .When(x => x.IsActive.HasValue);
        }
    }
}

using CRM_Homestay.Contract.FAQs;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class FAQValidator : AbstractValidator<CreateUpdateFAQDto>
    {
        public FAQValidator(Localizer L)
        {
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(3)
                .WithMessage(L["Validator.TooShort", 3])
                .MaximumLength(200)
                .WithMessage(L["Validator.TooLong", 200]);

            RuleFor(x => x.Answer)
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

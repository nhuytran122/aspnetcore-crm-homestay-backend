using CRM_Homestay.Contract.CustomerAccounts;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class ProfileValidator : AbstractValidator<ProfileRequestDto>
{
    public ProfileValidator(Localizer L)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Name)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(1)
            .WithMessage(L["Validator.TooShort", 1])
            .MaximumLength(30)
            .WithMessage(L["Validator.TooMany", 30]);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Name)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(1)
            .WithMessage(L["Validator.TooShort", 1])
            .MaximumLength(30)
            .WithMessage(L["Validator.TooMany", 30]);

        RuleFor(x => x.Email)
            .Matches(InputRegularExpression.Email)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.DOB)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Must((x) => x!.Value.Kind == DateTimeKind.Unspecified)
            .When(x => x.DOB.HasValue)
            .WithMessage(L["Validator.InValidFormat"])
            .Must(x => x!.Value.Date <= DateTime.UtcNow.Date.AddYears(-13))
            .When(x => x.DOB.HasValue)
            .WithMessage(L["Validator.MustBeAtLeast10YearsOld"]);
    }
}
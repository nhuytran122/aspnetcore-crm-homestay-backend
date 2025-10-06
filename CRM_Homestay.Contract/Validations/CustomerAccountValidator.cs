using CRM_Homestay.Contract.CustomerAccounts;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class CustomerAccountValidator : AbstractValidator<SignUpRequestDto>
{
    public CustomerAccountValidator(Localizer L)
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.NumberPhone)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.Email)
            .Matches(InputRegularExpression.Email)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage(L["Validator.InValidFormat"]);

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

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Password)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(6)
            .WithMessage(L["Validator.TooShort", 6]);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage(L["Validator.NotEqualPassword"])
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"]);

        RuleFor(x => x.Gender)
            .Must(value => Enum.TryParse<Gender>(value, true, out _))
            .WithMessage(L["Validator.InvalidGender"]);
    }
}

public class CustomerAccountSignInValidator : AbstractValidator<SignInRequestDto>
{
    public CustomerAccountSignInValidator(Localizer L)
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.NumberPhone)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Password)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(6)
            .WithMessage(L["Validator.TooShort", 6]);
    }
}
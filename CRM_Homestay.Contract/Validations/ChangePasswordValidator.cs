using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator(Localizer L)
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Password)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(6)
            .WithMessage(L["Validator.TooShort", 6]);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Password)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(6)
            .WithMessage(L["Validator.TooShort", 6]);

        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.NewPassword)
            .WithMessage(L["Validator.NotEqualPassword"])
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"]);
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator(Localizer L)
    {

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Password)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(6)
            .WithMessage(L["Validator.TooShort", 6]);

        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.NewPassword)
            .WithMessage(L["Validator.NotEqualPassword"])
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"]);
    }
}
using CRM_Homestay.Contract.Users;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;


public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator(Localizer L)
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Word)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(5)
            .WithMessage(L["Validator.TooShort", 5]);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Password)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(6)
            .WithMessage(L["Validator.TooShort", 6]);
        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.Password)
            .WithMessage(L["Validator.NotEqualPassword"])
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"]);

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

        RuleFor(x => x.DOB)
            .Must((x) => x!.Value.Kind == DateTimeKind.Unspecified)
            .When(x => x.DOB.HasValue)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(InputRegularExpression.NumberPhone)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.Email)
            .Matches(InputRegularExpression.Email)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage(L["Validator.InValidFormat"]);


        RuleFor(x => x.RoleId)
            .Must((x) => x != Guid.Empty)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.Address.ProvinceId)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        RuleFor(x => x.Address.DistrictId)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        RuleFor(x => x.Address.WardId)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        RuleFor(x => x.Address.Street)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);

        RuleFor(x => x.BaseSalary)
            .GreaterThanOrEqualTo(0)
            .WithMessage(L["Validator.InValidFormat"]);

    }
}

public class UpdateUserProfileValidator : AbstractValidator<UpdateProfileRequestDto>
{
    public UpdateUserProfileValidator(Localizer L)
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


        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(InputRegularExpression.NumberPhone)
            .WithMessage(L["Validator.InValidFormat"]);


        RuleFor(x => x.Email)
            .Matches(InputRegularExpression.Email)
            .WithMessage(L["Validator.InValidFormat"])
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.DOB)
            .Must((x) => x!.Value.Kind == DateTimeKind.Unspecified)
            .When(x => x.DOB.HasValue)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.Address.ProvinceId)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        RuleFor(x => x.Address.DistrictId)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        RuleFor(x => x.Address.WardId)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        RuleFor(x => x.Address.Street)
            .NotEmpty().WithMessage(L["Validator.IsRequired"]);

    }

}
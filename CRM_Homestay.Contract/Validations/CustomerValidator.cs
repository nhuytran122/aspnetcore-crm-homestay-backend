using CRM_Homestay.Contract.Customers;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class CustomerValidator : AbstractValidator<CreateUpdateCustomerDto>
{
    public CustomerValidator(Localizer L)
    {
        RuleFor(x => x.Type)
                .Must(value => Enum.IsDefined(typeof(CustomerTypes), value))
                .WithMessage(L["Validator.EnumInvalid"]);
        RuleFor(x => x.Gender)
                .Must(value => Enum.IsDefined(typeof(Gender), value))
                .WithMessage(L["Validator.EnumInvalid"]);
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Name)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(1)
            .WithMessage(L["Validator.TooShort", 1])
            .MaximumLength(30)
            .WithMessage(L["Validator.TooMany", 30])
            .When(x => x.Type == CustomerTypes.Individual);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Name)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(1)
            .WithMessage(L["Validator.TooShort", 1])
            .MaximumLength(30)
            .WithMessage(L["Validator.TooMany", 30])
            .When(x => x.Type == CustomerTypes.Individual);

        //RuleFor(x => x.PhoneNumber)
        //    .NotEmpty()
        //    .Matches(InputRegularExpression.NumberPhone)
        //    .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.Email)
            .Matches(InputRegularExpression.Email)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.NotSpecialCharacter)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(2)
            .WithMessage(L["Validator.TooShort", 2])
            .When(x => x.Type == CustomerTypes.Organization);

        RuleFor(x => x.TaxCode)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Digit)
            .WithMessage(L["Validator.InValidFormat"])
            .When(x => x.Type == CustomerTypes.Organization);

        RuleFor(x => x.DOB)
            .Must((x) => x!.Value.Kind == DateTimeKind.Unspecified)
            .When(x => x.DOB.HasValue)
            .WithMessage(L["Validator.InValidFormat"]);

        //RuleFor(x => x.Address.ProvinceId)
        //    .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        //RuleFor(x => x.Address.DistrictId)
        //    .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        //RuleFor(x => x.Address.WardId)
        //    .NotEmpty().WithMessage(L["Validator.IsRequired"]);
        //RuleFor(x => x.Address.Street)
        //    .NotEmpty().WithMessage(L["Validator.IsRequired"]);
    }

}
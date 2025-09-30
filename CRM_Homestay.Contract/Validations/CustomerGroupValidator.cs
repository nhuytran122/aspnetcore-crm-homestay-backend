using CRM_Homestay.Contract.CustomerGroups;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class CustomerGroupValidator : AbstractValidator<CreateUpdateCustomerGroupDto>
{
    public CustomerGroupValidator(Localizer L)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Name)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(3)
            .WithMessage(L["Validator.TooShort", 3]);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Word)
            .WithMessage(L["Validator.InValidFormat"])
            .MinimumLength(3)
            .WithMessage(L["Validator.TooShort", 3]);

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage(L["Validator.TooLong", 255]);

        RuleFor(x => x.IsActive)
            .Must(x => x == true || x == false)
            .WithMessage(L["Validator.IsRequired"]);

        RuleFor(x => x.MinPoints.ToString())
            .Matches(InputRegularExpression.Numeric)
            .WithMessage(L["Validator.InValidFormat"]);
        RuleFor(x => x.MinPoints)
            .NotEmpty().WithMessage(L["Validator.IsRequired"])
            .GreaterThanOrEqualTo(0).WithMessage(L["Validator.GreaterThan", 0]);

        RuleFor(x => x.DiscountValue.ToString())
            .Matches(InputRegularExpression.Numeric)
            .WithMessage(L["Validator.InValidFormat"])
            .When(x => x.DiscountValue.HasValue);
        // RuleFor(x => x.DiscountValue)
        //     .GreaterThan(1).WithMessage(L["Validator.GreaterThan", 1]);

        RuleFor(x => x.DiscountType)
                // .NotNull()
                // .WithMessage(L["Validator.IsRequired"])
                .Must(value => value.HasValue && Enum.IsDefined(typeof(DiscountTypes), value))
                .WithMessage(L["Validator.EnumInvalid"])
                .When(x => x.DiscountType.HasValue);
        RuleFor(x => x)
        .Must(x =>
        {
            if (x.MinPoints > 0)
            {
                if (!x.DiscountType.HasValue || !x.DiscountValue.HasValue)
                    return false;

                if (x.DiscountType == DiscountTypes.Percentage)
                    return x.DiscountValue > 0 && x.DiscountValue <= 100;

                if (x.DiscountType == DiscountTypes.FixedAmount)
                    return x.DiscountValue > 0;
            }
            return true;
        })
        .WithMessage(x =>
        {
            if (x.MinPoints > 0)
            {
                if (!x.DiscountType.HasValue || !x.DiscountValue.HasValue)
                    return L["Validator.IsRequired"];

                if (x.DiscountType == DiscountTypes.Percentage)
                    return L[CouponErrorCode.PercentMustBeBetween, 1, 100];

                if (x.DiscountType == DiscountTypes.FixedAmount)
                    return L["Validator.GreaterThan", 0];
            }
            return L["Validator.InValidFormat"];
        });

    }
}
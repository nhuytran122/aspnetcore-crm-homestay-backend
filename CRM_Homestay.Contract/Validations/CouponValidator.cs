using CRM_Homestay.Contract.Coupons;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class CouponValidator : AbstractValidator<CreateUpdateCouponDto>
{
    public CouponValidator(Localizer L)
    {

        RuleFor(x => x.DiscountType)
            .NotNull()
            .WithMessage(L["Validator.IsRequired"])
            .Must(value => Enum.IsDefined(typeof(DiscountTypes), value!))
            .WithMessage(L["Validator.EnumInvalid"]);

        RuleFor(x => x.DiscountValue.ToString())
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Numeric)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.DiscountType == DiscountTypes.Percentage)
                    return x.DiscountValue > 0 && x.DiscountValue <= 100;
                if (x.DiscountType == DiscountTypes.FixedAmount)
                    return x.DiscountValue > 0;
                return true;
            })
            .WithMessage(x =>
            {
                if (x.DiscountType == DiscountTypes.Percentage)
                    return L[CouponErrorCode.PercentMustBeBetween, 1, 100];
                if (x.DiscountType == DiscountTypes.FixedAmount)
                    return L["MustBeGreaterThanZero"];
                return L["Validator.InValidFormat"];
            });

        RuleFor(x => x.TotalUsageLimit.ToString())
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"])
            .Matches(InputRegularExpression.Numeric)
            .WithMessage(L["Validator.InValidFormat"]);

        RuleFor(x => x.TotalUsageLimit)
            .GreaterThanOrEqualTo(1)
            .WithMessage(L["MustBeGreaterThanZero"]);

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.StartDate.HasValue && x.EndDate.HasValue)
                    return x.EndDate >= x.StartDate;
                return true;
            })
            .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);
    }
}

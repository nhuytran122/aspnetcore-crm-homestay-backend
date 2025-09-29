using CRM_Homestay.Contract.Branches;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class BranchValidator : AbstractValidator<CreateUpdateBranchDto>
    {
        public BranchValidator(Localizer L)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(3)
                .WithMessage(L["Validator.TooShort", 3])
                .MaximumLength(200)
                .WithMessage(L["Validator.TooLong", 200])
                .Matches(InputRegularExpression.NotSpecialCharacter);

            RuleFor(x => x.Address.ProvinceId)
                .NotEmpty().WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.Address.DistrictId)
                .NotEmpty().WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.Address.WardId)
                .NotEmpty().WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.Address.Street)
                .NotEmpty().WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .Matches(InputRegularExpression.NumberPhone)
                .WithMessage(L["Validator.InValidFormat"]);

            RuleFor(x => x.GatePassword)
                .NotEmpty().WithMessage(L["Validator.IsRequired"])
                .MinimumLength(4)
                .WithMessage(L["Validator.TooShort", 4])
                .MaximumLength(8)
                .WithMessage(L["Validator.TooLong", 8])
                .Matches(InputRegularExpression.Digit);

            RuleFor(x => x.Status)
                .NotNull().WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.IsMainBranch)
                .Must(x => x == true || x == false)
                .WithMessage(L["Validator.IsRequired"]);
        }
    }
}

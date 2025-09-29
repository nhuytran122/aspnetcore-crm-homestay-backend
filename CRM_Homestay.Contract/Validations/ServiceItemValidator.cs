using CRM_Homestay.Contract.HomestayServices;
using CRM_Homestay.Contract.ServiceItems;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class CreateUpdateServiceItemInServiceValidator : AbstractValidator<CreateUpdateServiceItemInServiceDto>
    {
        public CreateUpdateServiceItemInServiceValidator(Localizer L)
        {
            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);
            RuleFor(x => x.Brand)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);
            RuleFor(x => x.Model)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(value => Enum.IsDefined(typeof(ServiceItemTypes), value))
                .WithMessage(L["Validator.EnumInvalid"]);

            RuleFor(x => x)
            .Must(x =>
            {
                if (x.Type != ServiceItemTypes.Others)
                {
                    return !string.IsNullOrWhiteSpace(x.Identifier)
                        && !string.IsNullOrWhiteSpace(x.Brand)
                        && !string.IsNullOrWhiteSpace(x.Model);
                }
                return true;
            })
            .WithMessage(L[ServiceItemErrorCode.IdentifierBrandModelRequiredForVehicle]);
        }
    }

    public class CreateServiceItemValidator : AbstractValidator<CreateServiceItemDto>
    {
        public CreateServiceItemValidator(Localizer L)
        {
            RuleFor(x => x.HomestayServiceId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);
            RuleFor(x => x.Brand)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);
            RuleFor(x => x.Model)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

            RuleFor(x => x.Type)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(value => Enum.IsDefined(typeof(ServiceItemTypes), value))
                .WithMessage(L["Validator.EnumInvalid"]);

            RuleFor(x => x)
            .Must(x =>
            {
                if (x.Type != ServiceItemTypes.Others)
                {
                    return !string.IsNullOrWhiteSpace(x.Identifier)
                        && !string.IsNullOrWhiteSpace(x.Brand)
                        && !string.IsNullOrWhiteSpace(x.Model);
                }
                return true;
            })
            .WithMessage(L[ServiceItemErrorCode.IdentifierBrandModelRequiredForVehicle]);
        }

        public class UpdateServiceItemValidator : AbstractValidator<UpdateServiceItemDto>
        {
            public UpdateServiceItemValidator(Localizer L)
            {
                RuleFor(x => x.Description)
                    .MaximumLength(255)
                    .WithMessage(L["Validator.TooLong", 255]);
                RuleFor(x => x.Brand)
                    .MaximumLength(255)
                    .WithMessage(L["Validator.TooLong", 255]);
                RuleFor(x => x.Model)
                    .MaximumLength(255)
                    .WithMessage(L["Validator.TooLong", 255]);

                RuleFor(x => x.Type)
                    .NotNull()
                    .WithMessage(L["Validator.IsRequired"])
                    .Must(value => Enum.IsDefined(typeof(ServiceItemTypes), value))
                    .WithMessage(L["Validator.EnumInvalid"]);

                RuleFor(x => x)
                .Must(x =>
                {
                    if (x.Type != ServiceItemTypes.Others)
                    {
                        return !string.IsNullOrWhiteSpace(x.Identifier)
                            && !string.IsNullOrWhiteSpace(x.Brand)
                            && !string.IsNullOrWhiteSpace(x.Model);
                    }
                    return true;
                })
                .WithMessage(L[ServiceItemErrorCode.IdentifierBrandModelRequiredForVehicle]);
            }
        }
    }
}

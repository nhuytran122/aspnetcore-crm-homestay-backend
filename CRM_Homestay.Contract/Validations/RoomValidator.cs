using System.Text.RegularExpressions;
using CRM_Homestay.Contract.Rooms;
using CRM_Homestay.Core.Consts.Regulars;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations
{
    public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomDtoValidator(Localizer L)
        {
            RuleFor(x => x.RoomTypeId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.BranchId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.RoomNumber.ToString())
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"])
                .Matches(InputRegularExpression.Numeric)
                .WithMessage(L["Validator.InValidFormat"]);

            RuleFor(x => x.IsActive)
                .Must(x => x == true || x == false)
                .WithMessage(L["Validator.IsRequired"]);
            RuleForEach(x => x.RoomAmenities!)
                .SetValidator(new CreateAmenityForRoomValidator(L));
        }
    }

    public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
    {
        public UpdateRoomDtoValidator(Localizer L)
        {
            RuleFor(x => x.RoomTypeId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.BranchId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.IsActive)
                .Must(x => x == true || x == false)
                .WithMessage(L["Validator.IsRequired"]);
            RuleForEach(x => x.RoomAmenities!)
                .SetValidator(new UpdateAmenityForRoomValidator(L));
        }
    }

    public class CreateAmenityForRoomValidator : AbstractValidator<CreateAmenityForRoomDto>
    {
        public CreateAmenityForRoomValidator(Localizer L)
        {
            RuleFor(x => x.AmenityId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Quantity)
                .Must(q => !q.HasValue || q >= 1)
                .WithMessage(L["Validator.GreaterThan", 1]);
        }
    }

    public class UpdateAmenityForRoomValidator : AbstractValidator<UpdateAmenityForRoomDto>
    {
        public UpdateAmenityForRoomValidator(Localizer L)
        {
            // RuleFor(x => x.AmenityId)
            //     .NotEmpty()
            //     .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Quantity)
                .Must(q => !q.HasValue || q >= 1)
                .WithMessage(L["Validator.GreaterThan", 1]);
        }
    }
}

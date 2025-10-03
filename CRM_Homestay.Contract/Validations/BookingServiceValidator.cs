using CRM_Homestay.Contract.BookingServices;
using CRM_Homestay.Localization;
using FluentValidation;

namespace CRM_Homestay.Contract.Validations;

public class CreateBookingServicesValidator : AbstractValidator<CreateBookingServicesDto>
{
    public CreateBookingServicesValidator(Localizer L)
    {
        RuleFor(x => x.Services)
            .NotEmpty()
            .WithMessage(L["Validator.IsRequired"]);

        RuleForEach(x => x.Services)
            .SetValidator(new CreateBookingServiceValidator(L));
    }

    public class CreateBookingServiceValidator : AbstractValidator<CreateBookingServiceDto>
    {
        public CreateBookingServiceValidator(Localizer L)
        {
            RuleFor(x => x.ServiceId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

            RuleForEach(x => x.Items)
                .SetValidator(new CreateBookingServiceItemValidator(L));
        }
    }

    public class CreateBookingServiceItemValidator : AbstractValidator<CreateBookingServiceItemDto>
    {
        public CreateBookingServiceItemValidator(Localizer L)
        {
            RuleFor(x => x.ServiceItemId)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(StartDate => StartDate >= DateTime.Now.Date)
                .WithMessage(L["Validator.EndDateMustBeInFuture"]);
            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(EndDate => EndDate >= DateTime.Now.Date)
                .WithMessage(L["Validator.EndDateMustBeInFuture"]);
            RuleFor(x => x)
                .Must(x =>
                {
                    if (x.StartDate.HasValue && x.EndDate.HasValue)
                    {
                        return x.EndDate.Value.Date >= x.StartDate.Value.Date;
                    }
                    return true;
                })
                .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);
        }
    }
}

public class UpdateBookingServicesValidator : AbstractValidator<UpdateBookingServiceDto>
{
    public UpdateBookingServicesValidator(Localizer L)
    {
        RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage(L["Validator.TooLong", 255]);

        RuleForEach(x => x.Items)
            .SetValidator(new UpdateBookingServiceItemValidator(L));
    }

    public class UpdateBookingServiceItemValidator : AbstractValidator<UpdateBookingServiceItemDto>
    {
        public UpdateBookingServiceItemValidator(Localizer L)
        {
            RuleFor(x => x.ServiceItemId)
                .NotEmpty()
                .WithMessage(L["Validator.IsRequired"]);

            RuleFor(x => x.ServiceItemId)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"]);
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(StartDate => StartDate >= DateTime.Now.Date)
                .WithMessage(L["Validator.EndDateMustBeInFuture"]);
            RuleFor(x => x.EndDate)
                .NotNull()
                .WithMessage(L["Validator.IsRequired"])
                .Must(EndDate => EndDate >= DateTime.Now.Date)
                .WithMessage(L["Validator.EndDateMustBeInFuture"]);
            RuleFor(x => x)
                .Must(x =>
                {
                    if (x.StartDate.HasValue && x.EndDate.HasValue)
                    {
                        return x.EndDate.Value.Date >= x.StartDate.Value.Date;
                    }
                    return true;
                })
                .WithMessage(L["Validator.EndDateMustBeAfterStartDate"]);
        }
    }

}

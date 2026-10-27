using FluentValidation;

namespace ApplicationEcommerce.Features.GuestCheckout.Commands;

public class GuestCheckoutCommandValidator : AbstractValidator<GuestCheckoutCommand>
{
    public GuestCheckoutCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CustomerPhone).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.AddressDetails).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
    }
}
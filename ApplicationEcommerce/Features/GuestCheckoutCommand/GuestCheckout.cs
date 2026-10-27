using MediatR;

namespace ApplicationEcommerce.Features.GuestCheckout.Commands;

public record GuestCheckoutItem(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    string? Size,
    string? CustomImageUrl = null);

public record GuestCheckoutCommand(
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string City,
    string AddressDetails,
    string? Notes,
    string PaymentMethod,
    string? DepositAccount,
    string? DepositReceiptUrl,
    string? PromoCode,
    decimal? ShippingCost,
    List<GuestCheckoutItem> Items) : IRequest<Guid>;
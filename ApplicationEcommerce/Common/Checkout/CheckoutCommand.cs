using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Common.Checkout
{

    public record CheckoutCommand(
    string UserId,
    Guid ShippingAddressId,
    Guid? PaymentMethodId,
    string? DepositAccount,
    string? DepositReceiptUrl) : IRequest<Guid>;
}

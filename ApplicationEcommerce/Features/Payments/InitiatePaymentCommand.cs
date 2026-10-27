using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Payments
{
    public record InitiatePaymentCommand(
    decimal Amount,
    string OrderId,
    string CustomerEmail,
    string CustomerName) : IRequest<string>;
}

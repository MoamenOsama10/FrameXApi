using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain_Ecommerce.Entity;
namespace ApplicationEcommerce.Features.Payments
{
    public record ProcessWebhookCommand(
    string Hmac,
    IDictionary<string, string> Data,
    bool Success,
    string MerchantOrderId) : IRequest;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Interfaces
{
    public interface IPaymentService
    {
        Task<string> InitiatePaymentAsync(decimal amount, string orderId, string customerEmail, string customerName);
        bool VerifyWebhookSignature(string hmac, IDictionary<string, string> data);
    }
}

using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using  InfrastructureEcommerce;
namespace InfrastructureEcommerce.Services
{
    public class PaymobService : IPaymentService
    {
        private readonly PaymobSettings _settings;
        private readonly HttpClient _httpClient;

        public PaymobService(IOptions<InfrastructureEcommerce.Services.PaymobSettings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/api/");
        }

        // لو Paymob رجّع خطأ نعرف السبب الحقيقي بدل KeyNotFoundException غامضة
        private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Paymob request failed ({(int)response.StatusCode}): {body}");
            }

            return await response.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<string> InitiatePaymentAsync(
            decimal amount, string orderId, string customerEmail, string customerName)
        {
            // تقريب مش قص: 314.999999 لازم تبقى 31500 قرش مش 31499
            var amountCents = ((long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero)).ToString();

            // الخطوة 1: Auth Token
            var authResponse = await _httpClient.PostAsJsonAsync("auth/tokens", new
            {
                api_key = _settings.ApiKey
            });
            var authResult = await ReadJsonAsync(authResponse);
            var authToken = authResult.GetProperty("token").GetString();

            // الخطوة 2: Create Order
            var orderResponse = await _httpClient.PostAsJsonAsync("ecommerce/orders", new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = amountCents,
                currency = "EGP",
                merchant_order_id = orderId,
                items = new object[] { }
            });
            var orderResult = await ReadJsonAsync(orderResponse);
            var paymobOrderId = orderResult.GetProperty("id").GetInt64();

            // الخطوة 3: Payment Key
            var safeName = string.IsNullOrWhiteSpace(customerName) ? "Customer Customer" : customerName.Trim();
            var nameParts = safeName.Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "Customer";

            var paymentKeyResponse = await _httpClient.PostAsJsonAsync("acceptance/payment_keys", new
            {
                auth_token = authToken,
                amount_cents = amountCents,
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = customerEmail,
                    floor = "NA",
                    first_name = firstName,
                    street = "NA",
                    building = "NA",
                    phone_number = "+201000000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "NA",
                    country = "EG",
                    last_name = lastName,
                    state = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(_settings.IntegrationId)
            });

            var paymentKeyResult = await ReadJsonAsync(paymentKeyResponse);
            var paymentToken = paymentKeyResult.GetProperty("token").GetString();

            return $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentToken}";
        }

        public bool VerifyWebhookSignature(string hmac, IDictionary<string, string> data)
        {
            if (string.IsNullOrWhiteSpace(hmac))
                return false;

            // ترتيب الحقول ده ثابت حسب توثيق Paymob لـ Transaction Processed Callback
            var orderedKeys = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data.pan", "source_data.sub_type", "source_data.type", "success"
            };

            var concatenated = string.Join("", orderedKeys.Select(k => data.ContainsKey(k) ? data[k] : ""));
            using var hmacSha512 = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HmacSecret));
            var hashBytes = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var computedHmac = Convert.ToHexString(hashBytes).ToLowerInvariant();

            // مقارنة بزمن ثابت عشان محدش يخمّن التوقيع من فروق الزمن
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHmac),
                Encoding.UTF8.GetBytes(hmac.Trim().ToLowerInvariant()));
        }
    }
}
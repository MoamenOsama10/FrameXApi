using ApplicationEcommerce.Features.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseApiController
    {
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest request)
        {
            var iframeUrl = await Mediator.Send(new InitiatePaymentCommand(
                request.Amount, request.OrderId, request.CustomerEmail, request.CustomerName));

            return Ok(new { iframeUrl });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromQuery] string? hmac, [FromBody] JsonElement payload)
        {
            if (string.IsNullOrWhiteSpace(hmac))
                return BadRequest();

            if (payload.ValueKind != JsonValueKind.Object
                || !payload.TryGetProperty("obj", out var obj)
                || obj.ValueKind != JsonValueKind.Object)
                return BadRequest();

            // نفرد الحقول اللي الـ HMAC محتاجها، والـ booleans بحروف صغيرة زي ما Paymob بيحسبها
            var flatData = new Dictionary<string, string>();

            foreach (var prop in obj.EnumerateObject())
            {
                if (prop.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
                    continue;

                flatData[prop.Name] = ScalarToString(prop.Value);
            }

            var merchantOrderId = "";

            if (obj.TryGetProperty("order", out var order) && order.ValueKind == JsonValueKind.Object)
            {
                if (order.TryGetProperty("id", out var paymobOrderId))
                    flatData["order"] = ScalarToString(paymobOrderId);

                if (order.TryGetProperty("merchant_order_id", out var moid))
                    merchantOrderId = ScalarToString(moid);
            }

            if (obj.TryGetProperty("source_data", out var source) && source.ValueKind == JsonValueKind.Object)
            {
                foreach (var key in new[] { "pan", "sub_type", "type" })
                {
                    if (source.TryGetProperty(key, out var value))
                        flatData[$"source_data.{key}"] = ScalarToString(value);
                }
            }

            var success = flatData.TryGetValue("success", out var s) && s == "true";

            await Mediator.Send(new ProcessWebhookCommand(hmac, flatData, success, merchantOrderId));

            return Ok();
        }

        private static string ScalarToString(JsonElement element) => element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Number => element.GetRawText(),
            _ => ""
        };
    }

    public record InitiatePaymentRequest(decimal Amount, string OrderId, string CustomerEmail, string CustomerName);
}
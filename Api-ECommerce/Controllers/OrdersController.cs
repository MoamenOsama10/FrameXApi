using ApplicationEcommerce.Common;
using ApplicationEcommerce.Common.Checkout;
using ApplicationEcommerce.Features.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApplicationEcommerce.Interfaces;
using ApplicationEcommerce.Features.GuestCheckout.Commands;

namespace Api_ECommerce.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var orderId = await Mediator.Send(
                new CheckoutCommand(
                    CurrentUserId,
                    request.ShippingAddressId,
                    request.PaymentMethodId,
                    request.DepositAccount,
                    request.DepositReceiptUrl));

            return Ok(new { orderId });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var result = await Mediator.Send(new GetOrdersListQuery("guest"));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetOrderByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        // رفع صورة إيصال التحويل
        [HttpPost("upload-receipt")]
        [AllowAnonymous]
        [RequestSizeLimit(12 * 1024 * 1024)]
        public Task<IActionResult> UploadReceipt(IFormFile file) =>
            SaveImageAsync(file, "receipts");

        // رفع صورة العميل للمنتج المخصص (Custom Frame)
        [HttpPost("upload-custom-image")]
        [AllowAnonymous]
        [RequestSizeLimit(12 * 1024 * 1024)]
        public Task<IActionResult> UploadCustomImage(IFormFile file) =>
            SaveImageAsync(file, "custom");

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteOrderCommand(id));
            return NoContent();
        }

        [HttpPost("guest-checkout")]
        [AllowAnonymous]
        public async Task<IActionResult> GuestCheckout([FromBody] GuestCheckoutCommand command)
        {
            var orderId = await Mediator.Send(command);
            return Ok(new { orderId });
        }

        // فحص الصورة (النوع والحجم والمحتوى) قبل الحفظ، وبعدين الحفظ
        private async Task<IActionResult> SaveImageAsync(IFormFile file, string folder)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            using var stream = file.OpenReadStream();
            ImageUploadValidator.Validate(stream, file.FileName, file.Length);

            var fileStorage = HttpContext.RequestServices.GetRequiredService<IFileStorageService>();
            var imageUrl = await fileStorage.SaveFileAsync(stream, file.FileName, folder);

            return Ok(new { imageUrl });
        }
    }

    public record CheckoutRequest(
        Guid ShippingAddressId,
        Guid? PaymentMethodId,
        string? DepositAccount,
        string? DepositReceiptUrl);
}
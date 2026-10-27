using ApplicationEcommerce.Features.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_ECommerce.Controllers
{
    [Authorize]
    public class CartController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await Mediator.Send(new GetCartQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
        {
            var cartId = await Mediator.Send(
                new AddItemToCartCommand(CurrentUserId, request.ProductVariantId, request.Quantity));
            return Ok(new { cartId });
        }

        [HttpPut("items/{itemId:guid}")]
        public async Task<IActionResult> UpdateItem(Guid itemId, [FromBody] UpdateItemRequest request)
        {
            await Mediator.Send(new UpdateCartItemCommand(itemId, request.Quantity));
            return NoContent();
        }

        [HttpDelete("items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid itemId)
        {
            await Mediator.Send(new RemoveCartItemCommand(itemId));
            return NoContent();
        }
    }

    public record AddItemRequest(Guid ProductVariantId, int Quantity);
    public record UpdateItemRequest(int Quantity);
}

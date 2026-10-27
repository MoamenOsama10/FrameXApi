using ApplicationEcommerce.Features.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_ECommerce.Controllers
{
    [Authorize]
    public class WishlistController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var result = await Mediator.Send(new GetWishlistQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpPost("{productId:guid}")]
        public async Task<IActionResult> Add(Guid productId)
        {
            await Mediator.Send(new AddToWishlistCommand(CurrentUserId, productId));
            return NoContent();
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> Remove(Guid productId)
        {
            await Mediator.Send(new RemoveFromWishlistCommand(CurrentUserId, productId));
            return NoContent();
        }
    }
}

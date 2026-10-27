using ApplicationEcommerce.Features.PromoCodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PromoCodesController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetPromoCodesListQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePromoCodeCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { id });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeletePromoCodeCommand(id));
            return NoContent();
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidatePromoCodeCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}

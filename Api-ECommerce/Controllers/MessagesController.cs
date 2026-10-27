using ApplicationEcommerce.Features.ContactMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetMessagesListQuery());
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteMessageCommand(id));
            return NoContent();
        }
    }
}

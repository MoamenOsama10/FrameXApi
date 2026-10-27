using ApplicationEcommerce.Features.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_ECommerce.Controllers
{
    [Authorize]
    public class ProfileController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await Mediator.Send(new GetProfileQuery(CurrentUserId));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request)
        {
            var addressId = await Mediator.Send(new AddAddressCommand(
                CurrentUserId, request.Label, request.Street, request.City, request.Country, request.IsDefault));
            return Ok(new { addressId });
        }
    }

    public record AddAddressRequest(string Label, string Street, string City, string Country, bool IsDefault);
}

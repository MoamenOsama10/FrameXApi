using ApplicationEcommerce.Common.AddReview;
using ApplicationEcommerce.Features.Products;
using Domain_Ecommerce.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_ECommerce.Controllers
{
    public class ProductsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] Guid? categoryId,
            [FromQuery] Gender? gender,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12)
        {
            var query = new GetProductsListQuery
            {
                CategoryId = categoryId,
                Gender = gender,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetProductByIdQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers([FromQuery] int count = 5)
        {
            var result = await Mediator.Send(new GetBestSellersQuery(count));
            return Ok(result);
        }

        [HttpPost("{id:guid}/reviews")]
        [Authorize]
        public async Task<IActionResult> AddReview(Guid id, [FromBody] AddReviewRequest request)
        {
            var reviewId = await Mediator.Send(new AddReviewCommand(id, CurrentUserId, request.Rating, request.Comment));
            return CreatedAtAction(nameof(GetById), new { id }, reviewId);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id) return BadRequest("Route id and body id do not match.");

            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteProductCommand(id));
            return NoContent();
        }
        [HttpPost("{id:guid}/image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = file.OpenReadStream();

            var command = new UploadProductImageCommand
            {
                ProductId = id,
                FileStream = stream,
                FileName = file.FileName
            };

            var imageUrl = await Mediator.Send(command);
            return Ok(new { imageUrl });
        }
    }

    public record AddReviewRequest(int Rating, string Comment);
}

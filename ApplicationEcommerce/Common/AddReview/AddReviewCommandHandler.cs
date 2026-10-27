using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationEcommerce.Common.Exceptions;
namespace ApplicationEcommerce.Common.AddReview
{
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;

        public AddReviewCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public async Task<Guid> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetWithVariantsAsync(request.ProductId)
                ?? throw new NotFoundException(nameof(Product), request.ProductId);

            var review = new Review
            {
                ProductId = request.ProductId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment
            };

            await _unitOfWork.Repository<Review>().AddAsync(review);

            // إعادة حساب الـ Rating و ReviewsCount (Stored Columns)
            var newReviewsCount = product.ReviewsCount + 1;
            var newRating = ((product.Rating * product.ReviewsCount) + request.Rating) / newReviewsCount;

            product.ReviewsCount = newReviewsCount;
            product.Rating = Math.Round(newRating, 2);

            _unitOfWork.Repository<Product>().Update(product);

            await _unitOfWork.SaveChangesAsync();

            return review.Id;
        }
    }
}

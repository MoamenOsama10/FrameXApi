using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Categories
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(Category), request.Id);

            // الـ Query Filter بتاع Product بيخلّي النتيجة دي للمنتجات النشطة بس
            var activeProducts = await _unitOfWork.Repository<Product>()
                .FindAsync(p => p.CategoryId == request.Id);

            if (activeProducts.Any())
                throw new ConflictException("لا يمكن حذف هذا التصنيف لأنه يحتوي على منتجات. انقل المنتجات أو احذفها أولاً.");

            // Soft delete
            category.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
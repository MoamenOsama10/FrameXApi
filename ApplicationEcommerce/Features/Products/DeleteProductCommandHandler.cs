using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Products
{

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
{
    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id)
        ?? throw new NotFoundException(nameof(Product), request.Id);

    // Soft delete: نقلب الـ flag بدل ما نمسح الـ row
    product.IsDeleted = true;

    await _unitOfWork.SaveChangesAsync();
}
    }
}

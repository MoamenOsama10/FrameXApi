using ApplicationEcommerce.Common.Exceptions;
using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.ContactMessages
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _unitOfWork.Repository<ContactMessage>().GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(ContactMessage), request.Id);

            // Soft delete
            message.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
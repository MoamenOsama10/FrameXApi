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
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message
            };

            await _unitOfWork.Repository<ContactMessage>().AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return message.Id;
        }
    }
}

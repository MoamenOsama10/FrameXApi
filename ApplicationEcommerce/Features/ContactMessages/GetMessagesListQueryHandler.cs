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
    public class GetMessagesListQueryHandler : IRequestHandler<GetMessagesListQuery, List<ContactMessageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMessagesListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ContactMessageDto>> Handle(GetMessagesListQuery request, CancellationToken cancellationToken)
        {
            var messages = await _unitOfWork.Repository<ContactMessage>().GetAllAsync();

            return messages
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new ContactMessageDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Message = m.Message,
                    CreatedAt = m.CreatedAt
                }).ToList();
        }
    }
}

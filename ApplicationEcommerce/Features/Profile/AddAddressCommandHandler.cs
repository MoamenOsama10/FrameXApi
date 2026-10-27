using ApplicationEcommerce.Interfaces;
using Domain_Ecommerce.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Profile
{

    public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddAddressCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddAddressCommand request, CancellationToken cancellationToken)
        {
            var address = new Address
            {
                UserId = request.UserId,
                Label = request.Label,
                Street = request.Street,
                City = request.City,
                Country = request.Country,
                IsDefault = request.IsDefault
            };

            await _unitOfWork.Repository<Address>().AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return address.Id;
        }
    }
}

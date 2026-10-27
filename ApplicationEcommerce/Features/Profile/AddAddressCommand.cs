using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Profile
{
    public record AddAddressCommand(
     string UserId,
     string Label,
     string Street,
     string City,
     string Country,
     bool IsDefault) : IRequest<Guid>;
}

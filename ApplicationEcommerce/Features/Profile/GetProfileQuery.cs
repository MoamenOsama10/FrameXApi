using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Profile
{
    public record GetProfileQuery(string UserId) : IRequest<ProfileDto?>;
}

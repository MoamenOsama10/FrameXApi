using ApplicationEcommerce.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Profile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto?>
    {
        private readonly IIdentityService _identityService;

        public GetProfileQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<ProfileDto?> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _identityService.GetUserProfileAsync(request.UserId);

            if (profile is null) return null;

            return new ProfileDto
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Gender = profile.Gender,
                DateOfBirth = profile.DateOfBirth,
                MembershipType = profile.MembershipType,
                MemberSince = profile.MemberSince
            };
        }
    }
}

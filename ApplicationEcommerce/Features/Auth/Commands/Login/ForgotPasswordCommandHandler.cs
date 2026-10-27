using ApplicationEcommerce.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Auth.Commands.Login
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IIdentityService _identityService;

        public ForgotPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            await _identityService.ForgotPasswordAsync(request.Email);
            // مفيش Throw لو الإيميل مش موجود، عشان ميبانش هل الإيميل ده مسجل عندنا أو لأ (Security)
        }
    }
}

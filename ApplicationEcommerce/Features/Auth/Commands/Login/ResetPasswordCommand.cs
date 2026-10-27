using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Auth.Commands.Login
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest;

}

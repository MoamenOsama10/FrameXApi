using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Auth.Commands.Register
{

    public record RegisterCommand(
        string FullName,
        string Email,
        string Password) : IRequest<string>; // بيرجع UserId
}

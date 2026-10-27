using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

    public record LoginResult(string AccessToken, string RefreshToken, string UserId);
}

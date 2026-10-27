using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Categories
{
    public record CreateCategoryCommand(string Name, string Slug) : IRequest<Guid>;
}

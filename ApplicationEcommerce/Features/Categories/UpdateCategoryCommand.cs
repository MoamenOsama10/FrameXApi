using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Categories
{
    public record UpdateCategoryCommand(Guid Id, string Name, string Slug) : IRequest;
}

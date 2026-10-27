using Domain_Ecommerce.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Products
{
   public record CreateProductCommand(
     string Name,
     string Description,
     Guid CategoryId,
     Gender Gender,
     decimal BasePrice,
     bool IsNew,
     bool IsOnSale,
     List<string>? Sizes,
     Dictionary<string, decimal>? SizePrices,
     List<string>? Colors = null,
     bool IsCustomizable = false
     ) : IRequest<Guid>;
     
}
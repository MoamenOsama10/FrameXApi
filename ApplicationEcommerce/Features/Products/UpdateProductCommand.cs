using Domain_Ecommerce.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Products
{
   public record UpdateProductCommand(
      Guid Id,
      string Name,
      string Description,
      Guid CategoryId,
      Gender Gender,
      decimal BasePrice,
      bool IsNew,
      bool IsOnSale,
      List<string>? Sizes,
      Dictionary<string, decimal>? SizePrices,
     bool IsCustomizable = false,
      List<string>? Colors = null) : IRequest;
}
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Common.AddReview
{
    public record AddReviewCommand(
     Guid ProductId,
     string UserId,
     int Rating,
     string Comment) : IRequest<Guid>;
}

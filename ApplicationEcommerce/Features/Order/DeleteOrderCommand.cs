using MediatR;

namespace ApplicationEcommerce.Features.Order
{
    public record DeleteOrderCommand(Guid Id) : IRequest;
}
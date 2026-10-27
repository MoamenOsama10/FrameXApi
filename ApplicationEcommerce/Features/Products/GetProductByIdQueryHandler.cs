using ApplicationEcommerce.DTOs;
using ApplicationEcommerce.Features.Products;
using ApplicationEcommerce.Interfaces;
using AutoMapper;
using MediatR;

namespace ApplicationEcommerce.Features.Products;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetWithVariantsAsync(request.Id);

        if (product is null)
            return null;

        // ترتيب ثابت للمقاسات: من الأصغر للأكبر (Small, Medium, Large)
        product.Variants = product.Variants.OrderBy(v => v.Size).ToList();

        return _mapper.Map<ProductDto>(product);
    }
}
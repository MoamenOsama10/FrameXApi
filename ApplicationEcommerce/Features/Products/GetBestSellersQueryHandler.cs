using ApplicationEcommerce.DTOs;
using ApplicationEcommerce.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Features.Products
{

    public class GetBestSellersQueryHandler : IRequestHandler<GetBestSellersQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetBestSellersQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetBestSellersQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetBestSellersAsync(request.Count);
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}

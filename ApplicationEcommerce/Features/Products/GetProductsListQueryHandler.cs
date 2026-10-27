using ApplicationEcommerce.Common.Models;
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
    public class GetProductsListQueryHandler
     : IRequestHandler<GetProductsListQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductsListQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> Handle(
            GetProductsListQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _productRepository.GetFilteredAsync(
                request.CategoryId,
                request.Gender,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.PageNumber,
                request.PageSize);

            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<List<ProductDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

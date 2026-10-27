using ApplicationEcommerce.DTOs;
using AutoMapper;
using Domain_Ecommerce.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));

            CreateMap<ProductVariant, ProductVariantDto>();
            CreateMap<VariantImage, VariantImageDto>();
        }
    }
}

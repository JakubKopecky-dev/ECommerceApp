using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CartService.Application.DTOs.Cart;
using CartService.Application.DTOs.CartItem;
using CartService.Domain.Entity;

namespace CartService.Application.Mapping
{
    public class AutomapperConfigurationProfile : Profile
    {

        public AutomapperConfigurationProfile()
        {

            CreateMap<CreateCartItemDto, CartItem>();
            CreateMap<CartItem, CartItemDto>();


            CreateMap<Cart, CartExtendedDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.UnitPrice * i.Quantity)));

            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.UnitPrice * i.Quantity)));

            CreateMap<Cart, CheckoutCartDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.UnitPrice * i.Quantity)));


        }
    }
}

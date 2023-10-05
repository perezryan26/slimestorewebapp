using AutoMapper;
using SlimeStoreWeb.Data;
using SlimeStoreWeb.Models;

namespace SlimeStoreWeb.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {

            CreateMap<Item, ItemViewModel>().ReverseMap();
            CreateMap<Item, ItemCreateViewModel>().ReverseMap();

            CreateMap<Category, CategoryViewModel>().ReverseMap();

            CreateMap<Cart, CartViewModel>().ReverseMap();
            CreateMap<CartItem, CartItemViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();
            CreateMap<Order, OrderCreateViewModel>().ReverseMap();

        }
    }
}

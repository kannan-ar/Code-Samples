using AutoMapper;

namespace LibraryApi.Features.Products;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<ProductEntity, ProductModel>();
    }
}

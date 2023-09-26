using AutoMapper;
using LibraryApi.Helpers;

namespace LibraryApi.Features.Products;

public class ProductService : IProductService
{
    private readonly IStorageClient client;
    private const string tableName = "Products";
    private readonly IMapper mapper;

    public ProductService(IStorageClient client, IMapper mapper)
    {
        this.client = client;
        this.mapper = mapper;
    }

    public async Task<ProductModel> Get(string key)
    {
        return mapper.Map<ProductModel>(await client.Get<ProductEntity>(tableName, key, key));
    }
}

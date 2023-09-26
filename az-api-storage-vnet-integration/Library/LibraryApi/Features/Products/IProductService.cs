namespace LibraryApi.Features.Products
{
    public interface IProductService
    {
        Task<ProductModel> Get(string key);
    }
}

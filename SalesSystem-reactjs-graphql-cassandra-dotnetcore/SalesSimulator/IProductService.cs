namespace SalesSimulator
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Models;
    public interface IProductService
    {
        Task<IList<Product>> GetProducts();
    }
}
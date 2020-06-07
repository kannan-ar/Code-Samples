namespace SalesSimulator
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Models;
    public interface IProductService
    {
        Task<IList<Product>> GetProducts();
        IEnumerable<Product> GetProductsByInterests(ReadOnlyCollection<Product> allProducts, IEnumerable<string> interests);
        IEnumerable<Product> GetProductsToBuy(IEnumerable<Product> products);
    }
}
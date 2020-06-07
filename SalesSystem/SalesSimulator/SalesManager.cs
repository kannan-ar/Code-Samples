namespace SalesSimulator
{
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Models;

    public sealed class SalesManager
    {
        private readonly IUserService userService;
        private readonly IProductService productService;
        public SalesManager(IUserService userService, IProductService productService)
        {
            this.userService = userService;
            this.productService = productService;
        }

        public void Buy(ReadOnlyCollection<User> users, ReadOnlyCollection<Product> products)
        {
            var activeUsers = userService.GetActiveUsersFromCollection(users);
            var list = activeUsers.ToList();

            Parallel.ForEach(activeUsers, user =>
            {
                var interests = userService.GetCurrentInterests(user);
                var productsByInterests = productService.GetProductsByInterests(products, interests);
                var productsToBy = productService.GetProductsToBuy(productsByInterests);

                if (productsToBy != null)
                {
                    System.Console.WriteLine(user.Name + ": " + string.Join(",", productsToBy.Select(p => p.Name)));
                }
            });
        }

        public async Task Run()
        {
            var users = new ReadOnlyCollection<User>(await userService.GetUsers());
            var products = new ReadOnlyCollection<Product>(await productService.GetProducts());

            while (true)
            {
                await Task.Run(() =>
                {
                    Buy(users, products);
                });
            }
        }
    }
}
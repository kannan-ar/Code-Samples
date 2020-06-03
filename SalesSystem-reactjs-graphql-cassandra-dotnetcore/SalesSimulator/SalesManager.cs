namespace SalesSimulator
{
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;

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

        public async Task Run()
        {
            var users = new ReadOnlyCollection<User>(await userService.GetUsers());
            var products = new ReadOnlyCollection<Product>(await productService.GetProducts());

            var activeUsers = userService.GetActiveUsersFromCollection(users);

            foreach (var user in activeUsers)
            {
                var interests = userService.GetCurrentInterests(user);
            }
        }
    }
}
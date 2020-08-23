namespace SalesSimulator
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Collections.Generic;
    using Models;

    public sealed class SalesApplication
    {
        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly ICarrierService carrierService;
        private readonly ISaleService saleService;
        private readonly Random random;
        public SalesApplication(
            IUserService userService,
            IProductService productService,
            ICarrierService carrierService,
            ISaleService saleService)
        {
            this.userService = userService;
            this.productService = productService;
            this.carrierService = carrierService;
            this.saleService = saleService;
            random = new Random();
        }

        private void TestBuy(ReadOnlyCollection<User> users,
            ReadOnlyCollection<Product> products,
            ReadOnlyCollection<Carrier> carriers)
        {
            User user = users[random.Next(0, users.Count)];

            var interests = userService.GetCurrentInterests(user).ToList();
            var productsByInterests = productService.GetProductsByInterests(products, interests).ToList();

            var productsToBy = productService.GetProductsToBuy(productsByInterests);

            if (productsToBy != null)
            {
                BuyOne(user, productsToBy, carriers);
            }
        }

        private void BuyOne(User user,
            IEnumerable<Product> products,
            ReadOnlyCollection<Carrier> carriers)
        {
            List<Task> tasks = new List<Task>();

            foreach (var product in products)
            {
                tasks.Add(Task.Run(() =>
                {
                    saleService.Buy(new Purchase
                    {
                        Product = product,
                        User = user,
                        Quantity = 1,
                        Payment = saleService.GetModeOfPayment(),
                        Carrier = carrierService.GetCarrier(carriers).Name,
                        PurchasedOn = DateTime.Now
                    });
                }));
            }

            Task.WhenAll(tasks);
        }

        public void Buy(ReadOnlyCollection<User> users,
            ReadOnlyCollection<Product> products,
            ReadOnlyCollection<Carrier> carriers)
        {
            var activeUsers = userService.GetActiveUsersFromCollection(users);
            var list = activeUsers.ToList();

            Parallel.ForEach(activeUsers, user =>
            {
                var interests = userService.GetCurrentInterests(user).ToList();
                var productsByInterests = productService.GetProductsByInterests(products, interests).ToList();

                var productsToBy = productService.GetProductsToBuy(productsByInterests);

                if (productsToBy != null)
                {
                    //Console.WriteLine(DateTime.Now.ToString());
                    BuyOne(user, productsToBy, carriers);
                }
            });
        }

        public async Task RunAsync()
        {
            var users = new ReadOnlyCollection<User>(await userService.GetUsers());
            var products = new ReadOnlyCollection<Product>(await productService.GetProducts());
            var carriers = new ReadOnlyCollection<Carrier>(await carrierService.GetCarriers());

            while (true)
            {
                await Task.Run(() =>
                {
                    Buy(users, products, carriers);
                });
            }

        }
    }
}
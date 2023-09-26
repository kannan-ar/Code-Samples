using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Features.Products
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<ProductModel>> Get(string key)
        {
            return Ok(await productService.Get(key));
        }
    }
}

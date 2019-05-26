using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.API.Model;
using Microsoft.AspNetCore.Mvc;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductCatalogServices _catalogServices;

        public ProductsController()
        {
            _catalogServices = ServiceProxy.Create<IProductCatalogServices>(
                new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"),
                new ServicePartitionKey(0));
        }
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> Get()
        {
            try
            {
                IEnumerable<Product> allProducts = await _catalogServices.GetAllProducts();
                return allProducts.Select(p => new ApiProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.Availability > 0
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] ApiProduct product)
        {
            var newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Availability = 100
            };
            await _catalogServices.AddProduct(newProduct);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

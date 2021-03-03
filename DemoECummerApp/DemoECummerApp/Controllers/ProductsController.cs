using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DemoECummerApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return new Product[]
            {
                new Product(1, "Oats", new decimal(3.07)),
                new Product(2, "Toothpaste", new decimal(10.89)),
                new Product(3, "Televison", new decimal(500.90))
            };
        }
    }
}

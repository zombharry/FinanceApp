using Microsoft.AspNetCore.Mvc;

namespace Item.Api.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult GetAllProduct()
        {
            return View();
        }

        public IActionResult GetProduct()
        {
            return View();
        }
        public IActionResult CreateProduct()
        {
            return null;
        }

        public IActionResult EditProduct()
        {
            return null;
        }

        public IActionResult DeleteProduct()
        {
            return null;
        }
    }
}

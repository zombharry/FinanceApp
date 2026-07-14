using Microsoft.AspNetCore.Mvc;

namespace Item.Api.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult GetAllCategory()
        {
            return View();
        }

        public IActionResult GetCategoryl()
        {
            return View();
        }

        public IActionResult CreateCategory()
        {
            return null;
        }

        public IActionResult EditCategory()
        {
            return null;
        }

        public IActionResult DeleteCategory()
        {
            return null;
        }
    }
}

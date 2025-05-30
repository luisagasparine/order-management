using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

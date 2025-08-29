using Microsoft.AspNetCore.Mvc;

namespace NEW.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

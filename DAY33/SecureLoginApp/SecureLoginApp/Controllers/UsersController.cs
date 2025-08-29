using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureLoginApp.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            ViewBag.Message = "Welcome, User! Here is your profile information.";
            return View();
        }
    }
}

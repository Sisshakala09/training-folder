using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureMvcAuth.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            ViewBag.Message = "Welcome, User! Here is your profile information.";
            return View();
        }
    }
}

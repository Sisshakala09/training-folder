using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureMvcAuth.Models;
using System.ComponentModel.DataAnnotations;

namespace SecureMvcAuth.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Test> _signInManager;
        private readonly UserManager<Test> _userManager;

        public AccountController(SignInManager<Test> signInManager, UserManager<Test> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login() => View();

        public class LoginViewModel
        {
            [Required] public string Username { get; set; }
            [Required, DataType(DataType.Password)] public string Password { get; set; }
            public bool RememberMe { get; set; }
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin")) return RedirectToAction("Dashboard", "Admin");
                    return RedirectToAction("Profile", "User");
                }
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}

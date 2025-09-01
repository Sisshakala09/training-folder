using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureShop.Models;
using SecureShop.ViewModels;

namespace SecureShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly SignInManager<ApplicationUser> _signInMgr;

        public AccountController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signInMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new ApplicationUser { UserName = vm.Username, Email = vm.Email };
            var result = await _userMgr.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            await _userMgr.AddToRoleAsync(user, "Customer");
            await _signInMgr.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) => View(new LoginVM { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _signInMgr.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded) return LocalRedirect(vm.ReturnUrl ?? "/");
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account locked due to multiple failed attempts. Try again later.");
                return View(vm);
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInMgr.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}

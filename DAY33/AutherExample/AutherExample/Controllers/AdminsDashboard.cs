using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutherExample.Controllers
{
    //[Authorize(Roles = "admin")]
    public class AdminsDashboard : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminsDashboard(RoleManager<IdentityRole> role, UserManager<IdentityUser> user)
        {
            _roleManager = role;
            _userManager = user;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ViewBag.Error = "Role name is required";
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    ViewBag.Message = $"Role '{roleName}' created successfully";
                }
                else
                {
                    ViewBag.Error = string.Join(", ", result.Errors);
                }
            }
            else
            {
                ViewBag.Error = "Role already exists";
            }

            return View();
        }

        [HttpGet]
        public IActionResult AddUserToRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Error = "User not found";
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                ViewBag.Error = "Role does not exist";
                return View();
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                ViewBag.Message = $"User {email} added to role {roleName}";
            }
            else
            {
                ViewBag.Error = result.Errors;
            }

            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

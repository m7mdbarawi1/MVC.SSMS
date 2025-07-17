using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SSMS.Models;
using System.Security.Claims;

namespace SSMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SSMSContext _context;

        public AccountController(SSMSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Build claims using database user
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.FullName ?? "User"),
    new Claim("UserID", user.UserId.ToString()),
    new Claim("UserType", user.UserType.ToString()),
    new Claim(ClaimTypes.Role, user.UserType.ToString()) // Optional if using role-based [Authorize(Roles = "x")]
};

            var identity = new ClaimsIdentity(claims, "SSMSAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("SSMSAuth", principal);

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("SSMSAuth");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Saleos.Entity.Data;

namespace Saleos.Controllers
{
    public class Login : Controller
    {
        public IActionResult Index(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] User user)
        {
            if (user.Username == "Admin" && user.Password == "Admin")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            }
            else RedirectToAction(nameof(Index));

            return Redirect(user.ReturnUrl ?? "/");
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync();
        }

        public class User
        {
            public string ReturnUrl { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
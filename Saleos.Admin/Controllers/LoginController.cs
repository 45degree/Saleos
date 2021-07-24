/*
 * Copyright 2021 45degree
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Saleos.Admin.Models;
using Saleos.DAO;
using Saleos.Entity.Services.IdentityService;

namespace Saleos.Controllers
{
    public class LoginController : Controller
    {
        private IIdentityService _identityService;

        public LoginController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] LoginPostModel model)
        {
            var loginDAO = new LoginDAO
            {
                Username = model.Username,
                Password = model.Password,
            };

            try
            {
                var user = await _identityService.Login(loginDAO);
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            return Redirect(model.ReturnUrl ?? "/");
        }

        public async Task<IActionResult> Logout()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            var username = HttpContext.User.Claims
                    .SingleOrDefault(x => x.Type == ClaimTypes.Name).Value;
            if(!await _identityService.IsLogin(username))
            {
                return RedirectToAction(nameof(Index));
            }

            await _identityService.Logout(username);
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

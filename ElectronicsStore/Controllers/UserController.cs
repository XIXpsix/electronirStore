using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ElectronicsStore.Domain.ViewModels;
using ElectronicsStore.BLL.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace ElectronicsStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _appEnvironment;

        public UserController(IAccountService accountService, IWebHostEnvironment appEnvironment)
        {
            _accountService = accountService;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userName = User.Identity.Name;
            var response = await _accountService.GetUser(userName);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                var user = response.Data;
                var model = new UserProfileViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CurrentAvatarPath = user.AvatarPath
                };
                return View(model);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                string avatarPath = null;

                // Обработка загрузки файла
                if (model.Avatar != null)
                {
                    string path = "/images/avatars/" + model.Avatar.FileName;
                    // Сохраняем файл в папку wwwroot/images/avatars
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await model.Avatar.CopyToAsync(fileStream);
                    }
                    avatarPath = path;
                }

                var response = await _accountService.EditProfile(User.Identity.Name, model, avatarPath);

                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    // Обновляем Claims (куки), чтобы имя и аватарка обновились в шапке без перелогина
                    var user = response.Data;
                    var claims = new List<Claim>
                    {
                        new(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        new(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                        new("Id", user.Id.ToString()),
                        new("AvatarPath", user.AvatarPath)
                    };
                    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                    return RedirectToAction("Profile");
                }

                ModelState.AddModelError("", response.Description);
            }
            return View("Profile", model);
        }
    }
}
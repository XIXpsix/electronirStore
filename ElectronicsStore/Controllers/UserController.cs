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
            // Убедимся, что папка для аватаров существует
            var avatarsPath = Path.Combine(_appEnvironment.WebRootPath, "images", "avatars");
            if (!Directory.Exists(avatarsPath))
            {
                Directory.CreateDirectory(avatarsPath);
            }

            if (ModelState.IsValid)
            {
                string avatarPath = null;

                // Обработка загрузки файла
                if (model.Avatar != null)
                {
                    // Создаем уникальное имя файла
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Avatar.FileName);
                    string fullPath = Path.Combine(avatarsPath, fileName);

                    // Относительный путь для сохранения в БД
                    avatarPath = "/images/avatars/" + fileName;

                    // Сохраняем файл
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.Avatar.CopyToAsync(fileStream);
                    }
                }

                // ВАЖНО: Мы используем старое имя пользователя (из текущих куки) для поиска пользователя, 
                // так как новое имя может быть еще не сохранено
                var response = await _accountService.EditProfile(User.Identity.Name, model, avatarPath);

                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    // Обновляем Claims (куки), чтобы имя и аватарка обновились в шапке
                    var user = response.Data;
                    var claims = new List<Claim>
                    {
                        new(ClaimsIdentity.DefaultNameClaimType, user.Name),
                        new(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                        new("Id", user.Id.ToString()),
                        new("AvatarPath", user.AvatarPath)
                    };
                    ClaimsIdentity id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                                                           ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                    // Перелогиниваем пользователя с обновленными данными
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                    return RedirectToAction("Profile");
                }

                ModelState.AddModelError("", response.Description);
            }
            // Если ModelState невалиден, возвращаем модель обратно в представление
            return View("Profile", model);
        }
    }
}
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        // (Здесь будет логика твоего друга)

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // (Здесь будет логика сохранения в БД)

                // Возвращаем обратно ВСЮ МОДЕЛЬ (с email, password и т.д.)
                return Ok(model);
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new { message = string.Join("\n", errors) });
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // (Здесь будет логика входа)

                // ✅ ИЗМЕНЕНИЕ: Мы возвращаем обратно МОДЕЛЬ ВХОДА
                return Ok(model);
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new { message = string.Join("\n", errors) });
        }
    }
}
using ElectronicsStore.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Entity;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var response = await _cartService.GetUserCart(userId);

        if (response.StatusCode == StatusCode.OK)
        {
            // Используем оператор объединения (??) для защиты от Null
            return View(response.Data ?? new List<CartItem>());
        }

        ModelState.AddModelError("", response.Description ?? "Неизвестная ошибка при загрузке корзины.");
        return View(new List<CartItem>());
    }

    [HttpPost]
    // Принимаем ID товара
    public async Task<IActionResult> Add(int productId)
    {
        var userIdString = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return RedirectToAction("Login", "Account");
        }

        await _cartService.AddToCart(userId, productId);
        return RedirectToAction("Index", "Cart"); // Перенаправляем в корзину
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        var userIdString = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return RedirectToAction("Login", "Account");
        }

        await _cartService.RemoveFromCart(userId, productId);
        return RedirectToAction("Index", "Cart");
    }
}
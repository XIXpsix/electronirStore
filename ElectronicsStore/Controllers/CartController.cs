using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ElectronicsStore.BLL.Interfaces;
using System;

namespace ElectronicsStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // ИСПРАВЛЕНИЕ: ?? ""
            var userName = User.Identity?.Name ?? "";

            var response = await _cartService.GetItems(userName);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            var userName = User.Identity?.Name ?? "";
            await _cartService.AddItem(userName, id);
            return RedirectToAction("Catalog", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Remove(Guid id)
        {
            var userName = User.Identity?.Name ?? "";
            await _cartService.RemoveItem(userName, id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Clear()
        {
            var userName = User.Identity?.Name ?? "";
            await _cartService.ClearCart(userName);
            return RedirectToAction("Index");
        }
    }
}
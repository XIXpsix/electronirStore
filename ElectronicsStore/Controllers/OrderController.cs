using ElectronicsStore.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    [Authorize] // Только для вошедших пользователей
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userName = User.Identity?.Name;
            var response = await _orderService.GetOrders(userName);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Create(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                ModelState.AddModelError("", "Укажите адрес доставки");
                return View();
            }

            var userName = User.Identity?.Name;
            var response = await _orderService.CreateOrder(userName, address);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                // Заказ создан, переходим на главную (или можно создать страницу "Спасибо за заказ")
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", response.Description);
            return View();
        }
    }
}
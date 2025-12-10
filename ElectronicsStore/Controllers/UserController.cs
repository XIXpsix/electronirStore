using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class UserController : Controller
{
    // В этом представлении будет форма для редактирования профиля
    public IActionResult Profile()
    {
        return View();
    }
}
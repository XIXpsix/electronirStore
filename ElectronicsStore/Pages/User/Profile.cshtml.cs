using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElectronicsStore.Pages.User
{
    public class ProfileModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "Профиль пользователя";
        }
    }
}
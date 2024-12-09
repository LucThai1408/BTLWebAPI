using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

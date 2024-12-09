using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

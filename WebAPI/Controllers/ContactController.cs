using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace CRMApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

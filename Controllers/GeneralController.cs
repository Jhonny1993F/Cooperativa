using Microsoft.AspNetCore.Mvc;

namespace Cooperativa.Controllers
{
    public class GeneralController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Cooperativa.Controllers
{
    public class AdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

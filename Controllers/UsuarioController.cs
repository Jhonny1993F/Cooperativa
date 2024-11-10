using Microsoft.AspNetCore.Mvc;

namespace Cooperativa.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

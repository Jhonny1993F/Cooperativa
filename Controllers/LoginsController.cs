using Cooperativa.Data;
using Cooperativa.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cooperativa.Controllers
{
    public class LoginsController : Controller
    {
        private readonly CooperativaContext _context;

        public LoginsController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Logins/Login
        public IActionResult Login()
        {
            return View();
        }

        // GET: Logins/LoginClientes
        public IActionResult LoginClientes()
        {
            return View();
        }

        // POST: Logins/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("socio,contraseña")] Login login)
        {
            var socio = await _context.Socios
                .FirstOrDefaultAsync(s => s.socio == login.socio);
            if (ModelState.IsValid)
            {
                //var socio = await _context.Socios
                //.FirstOrDefaultAsync(s => s.socio == login.socio);

                if (socio != null && socio.socioID == 2 && socio.contraseña == login.contraseña)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, socio.socio),
                        new Claim(ClaimTypes.NameIdentifier, socio.socioID.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Administrador");  // Redirige a la página principal u otra
                  //return RedirectToAction("Details", "Socios", new { id = socio.socioID }); // segunda forma
                  //return RedirectToRoute(new { controller = "Socios", action = "Details", id = socio.socioID }); // tercera forma
                }

                if (socio != null && socio.contraseña == login.contraseña)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, socio.socio),
                        new Claim(ClaimTypes.NameIdentifier, socio.socioID.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "General");  // Redirige a la página principal u otra
                  //return RedirectToAction("Details", "Socios", new { id = socio.socioID }); // segunda forma
                  //return RedirectToRoute(new { controller = "Socios", action = "Details", id = socio.socioID }); // tercera forma
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                }
            }

            return View(login);
        }

        // POST: Logins/LoginClientes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginClientes([Bind("cliente,contraseña")] Login loginClientes)
        {
            if (ModelState.IsValid)
            {
                var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.cliente == loginClientes.cliente);
                if (cliente != null && cliente.contraseña == loginClientes.contraseña)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, cliente.cliente),
                        new Claim(ClaimTypes.NameIdentifier, cliente.clienteID.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Usuario");  // Redirige a la página principal u otra
                  //return RedirectToAction("Details", "Socios", new { id = socio.socioID }); // segunda forma
                  //return RedirectToRoute(new { controller = "Socios", action = "Details", id = socio.socioID }); // tercera forma
                }

                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                //}

                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                }
            }

            return View(loginClientes);
        }

        // GET: Logins/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Logins");
        }
    }
}



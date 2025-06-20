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

        // POST: Logins/Login (Para Socios)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("socio,contraseña")] Login login)
        {
            var socio = await _context.Socios
                .FirstOrDefaultAsync(s => s.socio == login.socio);

            if (ModelState.IsValid)
            {
                if (socio != null && socio.contraseña == login.contraseña)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, socio.socio),
                        new Claim(ClaimTypes.NameIdentifier, socio.socioID.ToString()),
                        new Claim("TipoUsuario", "Socio") // Nuevo claim
                    };

                    // Si el socioID es 2, se agrega el claim de admin
                    if (socio.socioID == 2)
                    {
                        claims.Add(new Claim("Admin", "Administrador"));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    if (socio.socioID == 2) // Si es el administrador
                        return RedirectToAction("Index", "Administrador");

                    return RedirectToAction("Index", "General");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                }
            }

            return View(login);
        }

        // POST: Logins/LoginClientes (Para Clientes)
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
                        new Claim(ClaimTypes.NameIdentifier, cliente.clienteID.ToString()),
                        new Claim("TipoUsuario", "Cliente") // Nuevo claim
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Usuario");
                }
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
            return RedirectToAction("Index", "Home");
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Cooperativa.Controllers
{
    public class ClientesController : Controller
    {
        private readonly CooperativaContext _context;

        public ClientesController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Socios
        public async Task<IActionResult> Index()
        {
            // Obtener el ID y el tipo de usuario (Socio o Cliente) desde los claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Obtienes el "TipoUsuario" (Socio o Cliente)

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }
            // Si es socio administrador (ID = 2)
            if (userRoleClaim == "Socio" && userId == 2)
            {
                // El administrador ve todos los ahorros
                var clientes = await _context.Clientes.ToListAsync();
                return View(clientes);
            }

            /*// Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var socio = await _context.Socios
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(socio);
            }*/

            // Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var cliente = await _context.Clientes
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(cliente);
            }

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del usuario autenticado y su rol (Socio o Cliente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Socio o Cliente

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "El cliente no existe";
                return RedirectToAction("Index");
            }

            // Buscar el ahorro con el ID proporcionado
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.clienteID == id);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "El cliente no existe";
                return RedirectToAction("Index");
            }

            var admin = await _context.Socios
                .FirstOrDefaultAsync(s => s.socioID == 2);

            if (admin == null)
            {
                return NotFound();
            }

            if (admin != null)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Cliente" && cliente.clienteID != userId))
                {
                    // Si el usuario no es socio, cerrar la sesión y redirigir al Home
                    await HttpContext.SignOutAsync(); // Esto cierra la sesión
                    TempData["ErrorMessage"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Index", "Home"); // Redirigir al Home
                }
            }

            if (userRoleClaim == "Cliente")
            {
                var clientes = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    //.Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                ViewData["cliente"] = new SelectList(clientes, "cliente", "cliente", cliente.clienteID);
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("clienteID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,cliente,inscripcion,correo,contraseña")] Clientes clientes)
        {
            if (!ValidarCedula(clientes.cedula))
            {
                ModelState.AddModelError("cedula", "La cédula ingresada no es válida.");
                return View(clientes);
            }

            if (ModelState.IsValid)
            {
                _context.Add(clientes);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cliente creado exitosamente";
                return RedirectToAction("Create");
            }
            return View(clientes);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del usuario autenticado y su rol (Socio o Cliente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Socio o Cliente

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "El cliente no existe";
                return RedirectToAction("Index");
            }

            // Buscar el ahorro con el ID proporcionado
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.clienteID == id);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "El cliente no existe";
                return RedirectToAction("Index");
            }

            var admin = await _context.Socios
                .FirstOrDefaultAsync(s => s.socioID == 2);

            if (admin == null)
            {
                return NotFound();
            }

            if (admin != null)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Cliente" && cliente.clienteID != userId))
                {
                    // Si el usuario no es socio, cerrar la sesión y redirigir al Home
                    await HttpContext.SignOutAsync(); // Esto cierra la sesión
                    TempData["ErrorMessage"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Index", "Home"); // Redirigir al Home
                }
            }

            if (userRoleClaim == "Cliente")
            {
                var clientes = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    //.Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                ViewData["cliente"] = new SelectList(clientes, "cliente", "cliente", cliente.clienteID);
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("clienteID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,cliente,inscripcion,correo,contraseña")] Clientes clientes)
        {
            if (id != clientes.clienteID)
            {
                return NotFound();
            }

            if (!ValidarCedula(clientes.cedula))
            {
                ModelState.AddModelError("cedula", "La cédula ingresada no es válida.");
                return View(clientes);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientes);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cliente actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientesExists(clientes.clienteID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(clientes);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Cliente no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del usuario autenticado y su rol (Socio o Cliente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Socio o Cliente

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "El cliente no existe";
                return RedirectToAction("Index");
            }

            // Buscar el ahorro con el ID proporcionado
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.clienteID == id);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "El cliente no existe";
                return RedirectToAction("Index");
            }

            var admin = await _context.Socios
                .FirstOrDefaultAsync(s => s.socioID == 2);

            if (admin == null)
            {
                return NotFound();
            }

            if (admin != null)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Cliente" && cliente.clienteID != userId))
                {
                    // Si el usuario no es socio, cerrar la sesión y redirigir al Home
                    await HttpContext.SignOutAsync(); // Esto cierra la sesión
                    TempData["ErrorMessage"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Index", "Home"); // Redirigir al Home
                }
            }

            if (userRoleClaim == "Cliente")
            {
                var clientes = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    //.Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                ViewData["cliente"] = new SelectList(clientes, "cliente", "cliente", cliente.clienteID);
            }
            return View(cliente);
    }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientes = await _context.Clientes.FindAsync(id);
            if (clientes != null)
            {
                _context.Clientes.Remove(clientes);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cliente eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private bool ClientesExists(int id)
        {
            return _context.Clientes.Any(e => e.clienteID == id);
        }

        public bool ValidarCedula(string cedula)
        {
            // Verificar que tenga 10 caracteres numéricos
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Length != 10 || !cedula.All(char.IsDigit))
            {
                return false;
            }

            // Obtener los primeros dos dígitos (provincia)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
            {
                return false; // Provincia inválida
            }

            // Obtener el tercer dígito (debe ser 0-6)
            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito < 0 || tercerDigito > 6)
            {
                return false; // Formato incorrecto
            }

            // Algoritmo de verificación Módulo 10
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int valor = int.Parse(cedula[i].ToString()) * coeficientes[i];
                if (valor >= 10)
                {
                    valor -= 9;
                }
                suma += valor;
            }

            int digitoVerificador = int.Parse(cedula[9].ToString());
            int decenaSuperior = (int)Math.Ceiling(suma / 10.0) * 10;
            int resultado = (decenaSuperior - suma) % 10;

            return resultado == digitoVerificador;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Cooperativa.Data;
using Cooperativa.Models;
using System.Security.Claims;
using System.Runtime.InteropServices;
using NuGet.Versioning;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Cooperativa.Controllers
{
    public class AhorrosController : Controller
    {
        private readonly CooperativaContext _context;

        public AhorrosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Ahorros
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
                var ahorros = await _context.Ahorros.ToListAsync();
                return View(ahorros);
            }

            // Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var ahorrosSocios = await _context.Ahorros
                .Where(a => a.socioID == userId)
                .ToListAsync();
                return View(ahorrosSocios);
            }

            // Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var ahorrosClientes = await _context.Ahorros
                .Where(a => a.clienteID == userId)
                .ToListAsync();
                return View(ahorrosClientes);
            }

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
        }

        // GET: Ahorros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Ahorro no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del usuario autenticado y su rol (Socio o Cliente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Socio o Cliente

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }

            // Buscar el ahorro con el ID proporcionado
            var ahorro = await _context.Ahorros.FirstOrDefaultAsync(a => a.ahorroID == id);

            if (ahorro == null)
            {
                TempData["ErrorMessage"] = "El ahorro no existe";
                return RedirectToAction("Index");
            }

            // Si el usuario es un socio administrador (ID = 2), puede ver todos los ahorros
            if (userRoleClaim == "Socio" && userId == 2)
            {
                return View(ahorro);
            }

            // Verificar si el usuario tiene permiso para ver el ahorro
            if ((userRoleClaim == "Socio" && ahorro.socioID == userId) ||
                (userRoleClaim == "Cliente" && ahorro.clienteID == userId))
            {
                return View(ahorro);
            }

            // Si el usuario no tiene permisos
            TempData["ErrorMessage"] = "No tienes permiso para ver este ahorro";
            return RedirectToAction("Index");
        }

        // GET: Ahorros/Create
        public async Task<IActionResult> Create()
        {
            // Obtener el ID y tipo de usuario logueado desde los claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // "Socio" o "Cliente"

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }

            // Listas para almacenar los datos del usuario autenticado
            List<Socios> socioLogueado = new List<Socios>();
            List<Clientes> clienteLogueado = new List<Clientes>();

            // Verificar el tipo de usuario y obtener los datos correspondientes
            if (userRoleClaim == "Socio")
            {
                socioLogueado = await _context.Socios
                    .Where(s => s.socioID == userId)
                    .ToListAsync();

                if (!socioLogueado.Any())
                {
                    TempData["ErrorMessage"] = "El socio no existe";
                    return RedirectToAction("Index");
                }
            }
            else if (userRoleClaim == "Cliente")
            {
                clienteLogueado = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    .ToListAsync();

                if (!clienteLogueado.Any())
                {
                    TempData["ErrorMessage"] = "El cliente no existe";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Rol de usuario desconocido";
                return RedirectToAction("Index");
            }

            // Llenar ViewData con el usuario autenticado (socio o cliente)
            ViewData["socio"] = new SelectList(socioLogueado, "socio", "socio");
            ViewData["cliente"] = new SelectList(clienteLogueado, "cliente", "cliente");

            return View();
        }

        // POST: Ahorros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("montoAhorro,comprobante,detalleAhorro,socio,cliente")] Ahorros ahorros, IFormFile comprobanteArchivo)
        {
            if (ModelState.IsValid)
            {
                // Asignar la fecha actual
                ahorros.fechaAhorro = DateTime.Now;

                // Obtener el ID y rol del usuario logueado desde los claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // "Socio" o "Cliente"

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    TempData["ErrorMessage"] = "Usuario no válido";
                    return RedirectToAction("Index");
                }

                // Verificar si el usuario autenticado es socio o cliente
                if (userRoleClaim == "Socio")
                {
                    var socio = await _context.Socios.FirstOrDefaultAsync(s => s.socioID == userId);
                    if (socio == null)
                    {
                        TempData["ErrorMessage"] = "El socio no existe";
                        return RedirectToAction("Index");
                    }
                    ahorros.socioID = socio.socioID; // Asignar el ID del socio logueado
                }
                else if (userRoleClaim == "Cliente")
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.clienteID == userId);
                    if (cliente == null)
                    {
                        TempData["ErrorMessage"] = "El cliente no existe";
                        return RedirectToAction("Index");
                    }
                    ahorros.clienteID = cliente.clienteID; // Asignar el ID del cliente logueado
                }
                else
                {
                    TempData["ErrorMessage"] = "Rol de usuario desconocido";
                    return RedirectToAction("Index");
                }

                /*// Guardar comprobante si se sube un archivo e la base de datos
                if (comprobanteArchivo != null && comprobanteArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await comprobanteArchivo.CopyToAsync(memoryStream);
                        ahorros.comprobante = memoryStream.ToArray(); // Guardar el archivo como bytes en la BD
                    }
                }*/

                // Guardar archivo en carpeta
                if (comprobanteArchivo != null && comprobanteArchivo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "comprobantes");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(comprobanteArchivo.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await comprobanteArchivo.CopyToAsync(fileStream);
                    }

                    ahorros.comprobante = "/comprobantes/" + uniqueFileName; // Guardar solo la ruta relativa
                }

                // Agregar el registro a la base de datos
                _context.Add(ahorros);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ahorro creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // En caso de error en el modelo, volver a cargar las listas
            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socio", "socio");
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "cliente", "cliente");

            return View(ahorros);
        }

        // GET: Ahorros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Ahorro no encontrado";
                return RedirectToAction("Index");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // "Socio" o "Cliente"

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }

            var ahorro = await _context.Ahorros
                .FirstOrDefaultAsync(a => a.ahorroID == id);

            if (ahorro == null)
            {
                TempData["ErrorMessage"] = "El ahorro no existe";
                return RedirectToAction("Index");
            }

            var admin = await _context.Socios
                .FirstOrDefaultAsync(s => s.socioID == 2);

            if (admin != admin)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Socio" && ahorro.socioID != userId) ||
                    (userRoleClaim == "Socio" && ahorro.socioID == userId) ||
                    (userRoleClaim == "Cliente" && ahorro.clienteID != userId) ||
                    (userRoleClaim == "Cliente" && ahorro.clienteID == userId)
                    )
                {
                    TempData["ErrorMessage"] = "No tienes permiso para editar este ahorro";
                    return RedirectToAction("Index");
                }
            }

            // Si el usuario es un administrador (ID = 2), permitirle elegir socios o clientes
            if (admin == admin)
            {
                var socios = await _context.Socios
                    .Where(s => s.socioID == userId)
                    .Select(s => new { Id = s.socioID, Nombre = s.socio })
                    .ToListAsync();

                var clientes = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    .Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                var usuarios = socios.Concat(clientes).ToList();
                ViewData["socio"] = new SelectList(usuarios, "Id", "Nombre", ahorro.socioID);
            }
            else if (userRoleClaim == "Socio")
            {
                var socioI = await _context.Socios
                    .Where(s => s.socioID == userId)
                    //.Select(s => new { Id = s.socioID, Nombre = s.socio })
                    .ToListAsync();

                ViewData["socio"] = new SelectList(socioI, "socio", "socio", ahorro.socioID);
            }
            else if (userRoleClaim == "Cliente")
            {
                var cliente = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    //.Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                ViewData["cliente"] = new SelectList(cliente, "cliente", "cliente", ahorro.clienteID);
            }

            return View(ahorro);
        }

        // POST: Ahorro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ahorroID,montoAhorro,comprobante,detalleAhorro")] Ahorros ahorros, IFormFile? nuevoComprobante)
        {
            if (id != ahorros.ahorroID)
            {
                TempData["ErrorMessage"] = "Ahorro no válido";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos inválidos";
                return View(ahorros);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // "Socio" o "Cliente"

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }

            var ahorroExistente = await _context.Ahorros.FindAsync(id);
            if (ahorroExistente == null)
            {
                TempData["ErrorMessage"] = "El ahorro no existe";
                return RedirectToAction("Index");
            }

            if (userId != 2)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Socio" && ahorroExistente.socioID != userId) ||
                    (userRoleClaim == "Cliente" && ahorroExistente.clienteID != userId))
                {
                    TempData["ErrorMessage"] = "No tienes permiso para editar este crédito";
                    return RedirectToAction("Index");
                }
            }

            // Actualizar solo los valores permitidos
            ahorroExistente.montoAhorro = ahorros.montoAhorro;
            ahorroExistente.comprobante = ahorros.comprobante;
            ahorroExistente.detalleAhorro = ahorros.detalleAhorro;

            /*// Si se sube un nuevo comprobante, actualizarlo en la base de datos
            if (nuevoComprobante != null && nuevoComprobante.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await nuevoComprobante.CopyToAsync(memoryStream);
                    ahorroExistente.comprobante = memoryStream.ToArray();
                }
            }*/

            // Si se sube un nuevo comprobante, actualizarlo
            if (nuevoComprobante != null && nuevoComprobante.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "comprobantes");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(nuevoComprobante.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await nuevoComprobante.CopyToAsync(fileStream);
                }

                ahorroExistente.comprobante = "/comprobantes/" + uniqueFileName; // Guardar solo la ruta relativa
            }

            try
            {
                _context.Update(ahorroExistente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ahorro actualizado correctamente";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Error al actualizar el ahorro";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Ahorros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Ahorro no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del usuario autenticado y su rol (Socio o Cliente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Socio o Cliente

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "El ahorro no existe";
                return RedirectToAction("Index");
            }

            // Buscar el ahorro con el ID proporcionado
            var ahorro = await _context.Ahorros.FirstOrDefaultAsync(a => a.ahorroID == id);

            if (ahorro == null)
            {
                TempData["ErrorMessage"] = "El ahorro no existe";
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
                if ((userRoleClaim == "Cliente" && ahorro.clienteID != userId) ||
                    (userRoleClaim == "Cliente" && ahorro.clienteID == userId))
                {
                    // Si el usuario no es socio, cerrar la sesión y redirigir al Home
                    await HttpContext.SignOutAsync(); // Esto cierra la sesión
                    TempData["ErrorMessage"] = "No tienes permisos para esta acción";
                    return RedirectToAction("Index", "Home"); // Redirigir al Home
                }
            }

            /*if (userRoleClaim == "Cliente")
            {
                var clientes = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    //.Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                ViewData["cliente"] = new SelectList(clientes, "cliente", "cliente", cliente.clienteID);
            }*/

            return View(ahorro);
        }

        // POST: Ahorros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ahorros == null)
            {
                return Problem("Entity set 'TextilCorpContext.Productos'  is null.");
            }

            var ahorros = await _context.Ahorros.FindAsync(id);
            if (ahorros != null)
            {
                _context.Ahorros.Remove(ahorros);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Ahorro eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private bool AhorrosExists(int id)
        {
            return (_context.Ahorros?.Any(e => e.ahorroID == id)).GetValueOrDefault();
        }
    }
}

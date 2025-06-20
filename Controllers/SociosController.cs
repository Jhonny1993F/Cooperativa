using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authentication;

namespace Cooperativa.Controllers
{
    //[Authorize]
    public class SociosController : Controller
    {
        private readonly CooperativaContext _context;

        public SociosController(CooperativaContext context)
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
                var socios = await _context.Socios.ToListAsync();
                return View(socios);
            }

            // Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var socio = await _context.Socios
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(socio);
            }

            /*// Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var cliente = await _context.Clientes
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(cliente);
            }*/

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
        }

        // GET: Socios/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Socio no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del socio autenticado
            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(socioIdClaim, out int socioId))
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return RedirectToAction("Index", "Socios");
            }

            var socio = await _context.Socios.FirstOrDefaultAsync(s => s.socioID == id);

            if (socio == null)
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return RedirectToAction("Index", "Socios");
            }

            if (socio.socioID != socioId)
            {
                TempData["ErrorMessage"] = "No tienes permiso para realizar esta accion";
                return RedirectToAction("Index", "Socios");
            }   
            return View(socio);
        }

        // GET: Socios/Create
        public IActionResult Create()
        {
            return View();
        }

        //// POST: Socios/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("socioID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,socio,inscripcion,correo,contraseña")] Socios socios)
        {
            if (!ValidarCedula(socios.cedula))
            {
                ModelState.AddModelError("cedula", "La cédula ingresada no es válida.");
                return View(socios);
            }

            if (ModelState.IsValid)
            {
                _context.Add(socios);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Socio creado exitosamente";

                // Si la petición es JSON, devuelve el objeto creado en formato JSON
                if (Request.Headers["Accept"] == "application/json")
                {
                    return Json(socios);
                }
                return RedirectToAction("Create");
            }
            return View(socios);
        }

        // GET: Socios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Socio no encontrado";
                return RedirectToAction("Index", "Socios");
            }

            // Obtener el ID del socio autenticado
            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(socioIdClaim, out int socioId))
            {
                // Verificar si el socio autenticado está intentando editar su propio perfil
                if (id != socioId)
                {
                    TempData["ErrorMessage"] = "No tienes permiso para realizar esta accion";
                    return RedirectToAction("Index", "Socios");
                }

                var socio = await _context.Socios.FindAsync(id);
                if (socio == null)
                {
                    TempData["ErrorMessage"] = "Socio no encontrado";
                    return RedirectToAction("Index", "Socios");
                }

                return View(socio);
            }
            else
            {
                //return Unauthorized(); // No autorizado si no se puede obtener el ID del socio autenticado
                return RedirectToAction("Index", "Socios");
            }
        }

        //// POST: Socios/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("socioID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,socio,inscripcion,correo,contraseña")] Socios socios)
        {
            // Obtener el ID del socio autenticado
            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(socioIdClaim, out int socioId))
            {
                // Validar que el usuario autenticado solo pueda editar su propio perfil
                if (id != socios.socioID || id != socioId)
                {
                    return Unauthorized();  // Si no coincide, se retorna "No Autorizado"
                }

                if (!ValidarCedula(socios.cedula))
                {
                    ModelState.AddModelError("cedula", "La cédula ingresada no es válida.");
                    return View(socios);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(socios);
                        await _context.SaveChangesAsync();

                        // Si es una petición JSON, devuelve el objeto actualizado
                        if (Request.Headers["Accept"] == "application/json")
                        {
                            return Json(socios);
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SociosExists(socios.socioID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    TempData["SuccessMessage"] = "Socio actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                return View(socios);
            }
            else
            {
                return Unauthorized(); // Si no se puede obtener el ID del socio autenticado
            }
        }

        // GET: Socios/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Socio no encontrado";
                return RedirectToAction("Index", "Socios");
            }

            // Obtener el ID del socio autenticado
            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(socioIdClaim, out int socioId))
            {
                // Verificar si el socio autenticado está intentando editar su propio perfil
                if (id != socioId)
                {
                    TempData["ErrorMessage"] = "No tienes permiso para realizar esta accion";
                    //return Unauthorized(); // No autorizado para editar otro usuario
                    return RedirectToAction("Index", "Socios");
                }

                var socio = await _context.Socios.FindAsync(id);
                if (socio == null)
                {
                    //return NotFound();
                    return RedirectToAction("Index", "Socios");
                }

                return View(socio);
            }
            else
            {
                //return Unauthorized(); // No autorizado si no se puede obtener el ID del socio autenticado
                return RedirectToAction("Index", "Socios");
            }

        }
        // Post Socios/Delete
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int socioID)
        {
            // Buscar el socio por ID
            var socio = await _context.Socios.FindAsync(socioID);
            if (socio == null)
            {
                return NotFound();
            }

            // Verificar si es el segundo clic para eliminar relaciones
            if (TempData["ConfirmarEliminacion"] != null && (bool)TempData["ConfirmarEliminacion"])
            {
                // Inicia una transacción para eliminar relaciones y luego al socio
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Eliminar relaciones en las tablas relacionadas
                    var utilidades = await _context.Utilidades.Where(u => u.socioID == socioID).ToListAsync();
                    _context.Utilidades.RemoveRange(utilidades);

                    var ahorros = await _context.Ahorros.Where(a => a.socioID == socioID).ToListAsync();
                    _context.Ahorros.RemoveRange(ahorros);

                    var creditos = await _context.Creditos.Where(p => p.socioID == socioID).ToListAsync();
                    _context.Creditos.RemoveRange(creditos);

                    var eventos = await _context.Eventos.Where(e => e.socioID == socioID).ToListAsync();
                    _context.Eventos.RemoveRange(eventos);

                    // Eliminar el socio
                    _context.Socios.Remove(socio);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "El socio y todas sus relaciones han sido eliminados correctamente.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Error al eliminar el socio: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            }

            // Primer clic: Verificar si tiene relaciones
            var tieneRelaciones = await _context.Utilidades.AnyAsync(u => u.socioID == socioID)
                                   || await _context.Ahorros.AnyAsync(a => a.socioID == socioID)
                                   || await _context.Creditos.AnyAsync(p => p.socioID == socioID)
                                   || await _context.Eventos.AnyAsync(e => e.socioID == socioID);

            if (tieneRelaciones)
            {
                TempData["WarningMessage"] = "Este socio tiene relaciones con otras tablas. Haz clic nuevamente en 'Eliminar' para confirmar la eliminación junto con todas sus relaciones.";
                TempData["ConfirmarEliminacion"] = true;
            }
            else
            {
                // Si no tiene relaciones, eliminar directamente
                _context.Socios.Remove(socio);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El socio ha sido eliminado correctamente.";
            }

            return RedirectToAction(nameof(Delete), new { id = socioID });
        }

        private bool SociosExists(int id)
        {
            return _context.Socios.Any(e => e.socioID == id);
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

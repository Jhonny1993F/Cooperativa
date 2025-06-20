using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Authentication;

namespace Cooperativa.Controllers
{
    public class CreditosController : Controller
    {
        private readonly CooperativaContext _context;

        public CreditosController(CooperativaContext context)
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
                var creditos = await _context.Creditos.ToListAsync();
                return View(creditos);
            }

            // Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var credito = await _context.Creditos
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(credito);
            }

            // Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var credito = await _context.Creditos
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(credito);
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
                TempData["ErrorMessage"] = "Credito no encontrado";
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
            var credito = await _context.Creditos.FirstOrDefaultAsync(a => a.creditoID == id);

            if (credito == null)
            {
                TempData["ErrorMessage"] = "El credito no existe";
                return RedirectToAction("Index");
            }

            // Si el usuario es un socio administrador (ID = 2), puede ver todos los ahorros
            if (userRoleClaim == "Socio" && userId == 2)
            {
                return View(credito);
            }

            // Verificar si el usuario tiene permiso para ver el ahorro
            if ((userRoleClaim == "Socio" && credito.socioID == userId) ||
                (userRoleClaim == "Cliente" && credito.clienteID == userId))
            {
                return View(credito);
            }

            // Si el usuario no tiene permisos
            TempData["ErrorMessage"] = "No tienes permiso para ver este credito";
            return RedirectToAction("Index");
        }

        // GET: Creditos/Create 
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
            ViewData["tipoCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "tipoCredito", "tipoCredito");

            return View();
        }

        // POST: Creditos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("creditoID,montoCredito,tipoCredito,tiempo,estado,socio,cliente")] Creditos creditos)
        {
            if (ModelState.IsValid)
            {
                // Asegúrate de que montoCredito y tiempo tengan valores válidos
                if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 0 && creditos.tiempo <= 3)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.025m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 3 && creditos.tiempo <= 6)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.015m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 6 && creditos.tiempo <= 9)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.0127m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 9 && creditos.tiempo <= 12)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.011m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "DIRECTO" && creditos.montoCredito >= 3000 && creditos.tiempo > 12 && creditos.tiempo <= 60)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.005m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "CONSUMO" && creditos.montoCredito >= 3000 && creditos.tiempo > 12 && creditos.tiempo <= 84)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.006m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "VIVIENDA HIPOTECARIA" && creditos.montoCredito >= 3000 && creditos.tiempo > 36 && creditos.tiempo <= 240)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.055m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else if (creditos.tipoCredito == "VIVIENDA DE INTERES PUBLICO" && creditos.montoCredito >= 3000 && creditos.tiempo > 36 && creditos.tiempo <= 300)
                {
                    // Calculando los valores
                    creditos.interes = Math.Round((creditos.montoCredito * 0.025m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                    creditos.totalCredito = creditos.montoCredito + creditos.interes;
                    creditos.cuota = creditos.totalCredito / creditos.tiempo;
                }
                else
                {
                    // Si no son válidos, asigna 0
                    creditos.interes = 0;
                    creditos.totalCredito = 0;
                    creditos.cuota = 0;
                }

                // Asignar la fecha actual
                creditos.fechaCredito = DateTime.Now;

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
                    creditos.socioID = socio.socioID; // Asignar el ID del socio logueado
                }
                else if (userRoleClaim == "Cliente")
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.clienteID == userId);
                    if (cliente == null)
                    {
                        TempData["ErrorMessage"] = "El cliente no existe";
                        return RedirectToAction("Index");
                    }
                    creditos.clienteID = cliente.clienteID; // Asignar el ID del cliente logueado
                }
                else
                {
                    TempData["ErrorMessage"] = "Rol de usuario desconocido";
                    return RedirectToAction("Index");
                }

                // Agregar el registro a la base de datos
                _context.Add(creditos);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credito creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // En caso de error en el modelo, volver a cargar las listas
            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "nombre");
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "nombre");
            ViewData["tipoCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "tipoCredito", "tipoCredito");

            return View(creditos);
        }

        // GET: Creditos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Crédito no encontrado";
                return RedirectToAction("Index");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // "Socio" o "Cliente"

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }

            var credito = await _context.Creditos
                .FirstOrDefaultAsync(c => c.creditoID == id);

            if (credito == null)
            {
                TempData["ErrorMessage"] = "El crédito no existe";
                return RedirectToAction("Index");
            }

            var admin = await _context.Socios
                .FirstOrDefaultAsync(s => s.socioID == 2);

            if(admin != admin)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Socio" && credito.socioID != userId) || 
                    (userRoleClaim == "Socio" && credito.socioID == userId) ||
                    (userRoleClaim == "Cliente" && credito.clienteID != userId) ||
                    (userRoleClaim == "Cliente" && credito.clienteID == userId)
                    )
                {
                    TempData["ErrorMessage"] = "No tienes permiso para editar este crédito";
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
                ViewData["socio"] = new SelectList(usuarios, "Id", "Nombre", credito.socioID);
                ViewData["tipoCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "tipoCredito", "tipoCredito");
            }
            else if (userRoleClaim == "Socio")
            {
                var socio = await _context.Socios
                    .Where(s => s.socioID == userId)
                    //.Select(s => new { Id = s.socioID, Nombre = s.socio })
                    .ToListAsync();

                ViewData["socio"] = new SelectList(socio, "socio", "socio", credito.socioID);
                ViewData["tipoCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "tipoCredito", "tipoCredito");
            }
            else if (userRoleClaim == "Cliente")
            {
                var cliente = await _context.Clientes
                    .Where(c => c.clienteID == userId)
                    //.Select(c => new { Id = c.clienteID, Nombre = c.cliente })
                    .ToListAsync();

                ViewData["cliente"] = new SelectList(cliente, "cliente", "cliente", credito.clienteID);
                ViewData["tipoCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "tipoCredito", "tipoCredito");
            }

            return View(credito);
        }

        // POST: Creditos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("creditoID,montoCredito,tipoCredito,tiempo,estado")] Creditos creditos)
        {
            if (id != creditos.creditoID)
            {
                TempData["ErrorMessage"] = "Crédito no válido";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos inválidos";
                return View(creditos);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // "Socio" o "Cliente"

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }

            var creditoExistente = await _context.Creditos.FindAsync(id);
            if (creditoExistente == null)
            {
                TempData["ErrorMessage"] = "El crédito no existe";
                return RedirectToAction("Index");
            }

            if(userId != 2)
            {
                // Verificar si el usuario autenticado tiene acceso al crédito
                if ((userRoleClaim == "Socio" && creditoExistente.socioID != userId) ||
                    (userRoleClaim == "Cliente" && creditoExistente.clienteID != userId))
                {
                    TempData["ErrorMessage"] = "No tienes permiso para editar este crédito";
                    return RedirectToAction("Index");
                }
            }

            // Validar que los valores sean mayores a cero
            if (creditos.montoCredito <= 0 || creditos.tiempo <= 0)
            {
                TempData["ErrorMessage"] = "El monto y el tiempo deben ser mayores a cero";
                return View(creditos);
            }

            // Actualizar solo los valores permitidos
            creditoExistente.montoCredito = creditos.montoCredito;
            creditoExistente.tipoCredito = creditos.tipoCredito;
            creditoExistente.tiempo = creditos.tiempo;
            creditoExistente.estado = creditos.estado;

            // Asegúrate de que montoCredito y tiempo tengan valores válidos
            if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 0 && creditos.tiempo <= 3)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.025m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 3 && creditos.tiempo <= 6)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.015m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 6 && creditos.tiempo <= 9)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.0127m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "NORMAL" && creditos.montoCredito > 0 && creditos.tiempo > 9 && creditos.tiempo <= 12)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.011m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "DIRECTO" && creditos.montoCredito >= 3000 && creditos.tiempo > 12 && creditos.tiempo <= 60)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.005m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "CONSUMO" && creditos.montoCredito >= 3000 && creditos.tiempo > 12 && creditos.tiempo <= 84)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.006m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "VIVIENDA HIPOTECARIA" && creditos.montoCredito >= 3000 && creditos.tiempo > 36 && creditos.tiempo <= 240)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.055m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else if (creditos.tipoCredito == "VIVIENDA DE INTERES PUBLICO" && creditos.montoCredito >= 3000 && creditos.tiempo > 36 && creditos.tiempo <= 300)
            {
                // Calculando los valores
                creditos.interes = Math.Round((creditos.montoCredito * 0.025m) * creditos.tiempo, 2); // Ejemplo de cálculo del interés
                creditos.totalCredito = creditos.montoCredito + creditos.interes;
                creditos.cuota = creditos.totalCredito / creditos.tiempo;
            }
            else
            {
                // Si no son válidos, asigna 0
                creditos.interes = 0;
                creditos.totalCredito = 0;
                creditos.cuota = 0;
            }

            try
            {
                _context.Update(creditoExistente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Crédito actualizado correctamente";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Error al actualizar el crédito";
            }
            ViewData["tipoCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "tipoCredito", "tipoCredito");
            return RedirectToAction(nameof(Index));
        }

        // GET: Creditos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Credito no encontrado";
                return RedirectToAction("Index");
            }

            // Obtener el ID del usuario autenticado y su rol (Socio o Cliente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Socio o Cliente

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "El credito no existe";
                return RedirectToAction("Index");
            }

            // Buscar el ahorro con el ID proporcionado
            var credito = await _context.Creditos.FirstOrDefaultAsync(c => c.creditoID == id);

            if (credito == null)
            {
                TempData["ErrorMessage"] = "El credito no existe";
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
                if ((userRoleClaim == "Cliente" && credito.clienteID != userId) ||
                    (userRoleClaim == "Cliente" && credito.clienteID == userId))
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

            return View(credito);
        }

        // POST: Creditos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var creditos = await _context.Creditos.FindAsync(id);
            if (creditos != null)
            {
                _context.Creditos.Remove(creditos);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Credito eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private bool CreditosExists(int id)
        {
            return _context.Creditos.Any(e => e.creditoID == id);
        }
    }
}

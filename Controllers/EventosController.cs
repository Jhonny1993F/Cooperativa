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
using NuGet.Protocol;
using Microsoft.AspNetCore.Authentication;

namespace Cooperativa.Controllers
{
    public class EventosController : Controller
    {
        private readonly CooperativaContext _context;

        public EventosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Eventos
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
                var eventos = await _context.Eventos.ToListAsync();
                return View(eventos);
            }

            // Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var evento = await _context.Eventos
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(evento);
            }

            /*// Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var evento = await _context.Eventos
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(evento);
            }*/

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
        }

        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Evento no encontrado";
                return RedirectToAction("Index");
            }

            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if(!int.TryParse(socioIdClaim, out int socioId))
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return RedirectToAction("Index");
            }

            var eventos = await _context.Eventos
                .FirstOrDefaultAsync(e => e.eventoID == id);

            if (eventos == null)
            {
                TempData["ErrorMessage"] = "El evento no existe";
                return RedirectToAction("Index");
            }

            if (eventos.socioID != socioId)
            {
                TempData["ErrorMessage"] = "No tienes permiso para esta accion";
                return RedirectToAction("Index");
            }

            return View(eventos);
        }

        // GET: Eventos/Create
        public async Task<IActionResult> Create()
        {
            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(socioIdClaim, out int socioId))
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return RedirectToAction("Index");
            }

            var socioLogueado = await _context.Socios
                .Where(s => s.socioID == socioId)
                .ToListAsync();
            
            if (!socioLogueado.Any())
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return View();
            }

            ViewData["socio"] = new SelectList(socioLogueado, "socio", "socio");
            return View();
        }

        // POST: Eventos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("eventoID,tipoEvento,costoEvento,detalleEvento,lugar,socio")] Eventos eventos)
        {
            if (ModelState.IsValid)
            {
                // Asignar la fecha actual
                eventos.fechaEvento = DateTime.Now;

                // Buscar el socio por su nombre
                if (!string.IsNullOrEmpty(eventos.socio))
                {
                    var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == eventos.socio);
                    if (socioSeleccionado != null)
                    {
                        eventos.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                    }
                    else
                    {
                        ModelState.AddModelError("socio", "El socio seleccionado no existe.");
                        //ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                        return View(eventos);
                    }
                }

                _context.Add(eventos);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Evento creado exitosamente";
                return RedirectToAction("Index");
                //return RedirectToAction(nameof(Index));
            }
            //ViewBag["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
            return View(eventos);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Evento no encontrado";
                return RedirectToAction("Index");
            }

            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(socioIdClaim, out int socioId))
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return RedirectToAction("Index");
            }

            var eventos = await _context.Eventos
                .FirstOrDefaultAsync(e => e.eventoID == id);

            if (eventos == null)
            {
                TempData["ErrorMessage"] = "El evento no existe";
                return RedirectToAction("Index");
            }

            if (eventos.socioID != socioId)
            {
                TempData["ErrorMessage"] = "No tienes permiso para esta accion";
                return RedirectToAction("Index");
            }

            var socioLogueado = await _context.Socios
                .Where(s => s.socioID == socioId)
                .ToListAsync();

            if (!socioLogueado.Any())
            {
                TempData["ErrorMessage"] = "Socio no permitido";
                return RedirectToAction("Index");
            }

            ViewData["socio"] = new SelectList(socioLogueado, "socio", "socio");
            return View(eventos);
        }

        // POST: Eventos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("eventoID,fechaEvento,tipoEvento,costoEvento,detalleEvento,lugar,socio")] Eventos eventos)
        {
            if (id != eventos.eventoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar el socio por su nombre
                    if (!string.IsNullOrEmpty(eventos.socio))
                    {
                        var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == eventos.socio);
                        if (socioSeleccionado != null)
                        {
                            eventos.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                        }
                        else
                        {
                            ModelState.AddModelError("socio", "El socio seleccionado no existe.");
                            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                            return View(eventos);
                        }
                    }

                    _context.Update(eventos);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Evento actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventosExists(eventos.eventoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(eventos);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Evento no encontrado";
                return RedirectToAction("Index");
            }

            var socioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(socioIdClaim, out int socioId))
            {
                TempData["ErrorMessage"] = "El socio no existe";
                return RedirectToAction("Index");
            }

            var eventos = await _context.Eventos
                .FirstOrDefaultAsync(e => e.eventoID == id);

            if (eventos == null)
            {
                TempData["ErrorMessage"] = "El evento no existe";
                return RedirectToAction("Index");
            }

            if (eventos.socioID != socioId)
            {
                TempData["ErrorMessage"] = "No tienes permiso para esta accion";
                return RedirectToAction("Index");
            }
            return View(eventos);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventos = await _context.Eventos.FindAsync(id);
            if (eventos != null)
            {
                _context.Eventos.Remove(eventos);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Evento eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private bool EventosExists(int id)
        {
            return _context.Eventos.Any(e => e.eventoID == id);
        }
    }
}

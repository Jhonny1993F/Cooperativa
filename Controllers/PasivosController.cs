using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Cooperativa.Controllers
{
    public class PasivosController : Controller
    {
        private readonly CooperativaContext _context;

        public PasivosController(CooperativaContext context)
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
                var pasivos = await _context.Pasivos.ToListAsync();
                return View(pasivos);
            }

            /*// Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var pasivo = await _context.Pasivos
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(pasivo);
            }

            // Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var pasivo = await _context.Pasivos
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(pasivo);
            }*/

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
        }

        // GET: Pasivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasivos = await _context.Pasivos
                .FirstOrDefaultAsync(m => m.pasivoID == id);
            if (pasivos == null)
            {
                return NotFound();
            }

            return View(pasivos);
        }

        // GET: Pasivos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pasivos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("pasivoID,tipo,costoPasivo,detalle")] Pasivos pasivos)
        {
            if (ModelState.IsValid)
            {
                pasivos.fechaPasivo = DateTime.Now;
                _context.Add(pasivos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pasivos);
        }

        // GET: Pasivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasivos = await _context.Pasivos.FindAsync(id);
            if (pasivos == null)
            {
                return NotFound();
            }
            return View(pasivos);
        }

        // POST: Pasivos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("pasivoID,tipo,costoPasivo,detalle,fechaPasivo")] Pasivos pasivos)
        {
            if (id != pasivos.pasivoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pasivos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PasivosExists(pasivos.pasivoID))
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
            return View(pasivos);
        }

        // GET: Pasivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasivos = await _context.Pasivos
                .FirstOrDefaultAsync(m => m.pasivoID == id);
            if (pasivos == null)
            {
                return NotFound();
            }

            return View(pasivos);
        }

        // POST: Pasivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pasivos = await _context.Pasivos.FindAsync(id);
            if (pasivos != null)
            {
                _context.Pasivos.Remove(pasivos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PasivosExists(int id)
        {
            return _context.Pasivos.Any(e => e.pasivoID == id);
        }
    }
}

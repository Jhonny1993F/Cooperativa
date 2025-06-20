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
    public class PasivosClientesController : Controller
    {
        private readonly CooperativaContext _context;

        public PasivosClientesController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Pasivos Clientes
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
                var pasivosC = await _context.PasivosClientes.ToListAsync();
                return View(pasivosC);
            }

            /*// Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var pasivoC = await _context.PasivosClientes
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(pasivoC);
            }

            // Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var pasivoC = await _context.PasivosClientes
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(pasivoC);
            }*/

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
        }

        // GET: PasivosClientes/Create
        public IActionResult Create()
        {
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente");
            return View();
        }

        // POST: PasivosClientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("pasivoID,tipo,costoPasivo,detallePasivo,cliente")] PasivosClientes pasivosClientes)
        {
            if (ModelState.IsValid)
            {
                pasivosClientes.fechaPasivo = DateTime.Now;

                if (!string.IsNullOrEmpty(pasivosClientes.cliente))
                {
                    var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == pasivosClientes.cliente);
                    if (clienteSeleccionado != null)
                    {
                        pasivosClientes.clienteID = clienteSeleccionado.clienteID;
                    }
                    else
                    {
                        ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                        ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
                        return View(pasivosClientes);
                    }
                }

                _context.Add(pasivosClientes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
            return View(pasivosClientes);
        }

        // GET: PasivosClientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasivosClientes = await _context.PasivosClientes.FindAsync(id);
            if (pasivosClientes == null)
            {
                return NotFound();
            }
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente", pasivosClientes.cliente);
            return View(pasivosClientes);
        }

        // POST: PasivosClientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("pasivoClienteID,tipo,costoPasivo,detallePasivo,fechaPasivo,cliente")] PasivosClientes pasivosClientes)
        {
            if (id != pasivosClientes.pasivoClienteID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(pasivosClientes.cliente))
                    {
                        var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == pasivosClientes.cliente);
                        if (clienteSeleccionado != null)
                        {
                            pasivosClientes.clienteID = clienteSeleccionado.clienteID;
                        }
                        else
                        {
                            ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
                            return View(pasivosClientes);
                        }
                    }

                    _context.Update(pasivosClientes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PasivosClientesExists(pasivosClientes.pasivoClienteID))
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
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente", pasivosClientes.clienteID);
            return View(pasivosClientes);
        }

        // GET: PasivosClientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasivosClientes = await _context.PasivosClientes
                .FirstOrDefaultAsync(m => m.pasivoClienteID == id);
            if (pasivosClientes == null)
            {
                return NotFound();
            }

            return View(pasivosClientes);
        }

        // POST: PasivosClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pasivosClientes = await _context.PasivosClientes.FindAsync(id);
            if (pasivosClientes != null)
            {
                _context.PasivosClientes.Remove(pasivosClientes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PasivosClientesExists(int id)
        {
            return _context.PasivosClientes.Any(e => e.pasivoClienteID == id);
        }
    }
}

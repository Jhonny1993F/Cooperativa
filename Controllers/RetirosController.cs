using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;

namespace Cooperativa.Controllers
{
    public class RetirosController : Controller
    {
        private readonly CooperativaContext _context;

        public RetirosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Retiros
        public async Task<IActionResult> Index()
        {
            var CooperativaContext = _context.Retiros.Include(c => c.clientes).Include(c => c.clientes);
            return View(await _context.Retiros.ToListAsync());
        }

        // GET: Retiros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retiros = await _context.Retiros
                .Include(c => c.clientes)
                .FirstOrDefaultAsync(m => m.retiroID == id);
            if (retiros == null)
            {
                return NotFound();
            }

            return View(retiros);
        }

        // GET: Retiros/Create
        public IActionResult Create()
        {
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente");
            return View();
        }

        // POST: Retiros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("retiroID,cantidadRetiro,detalleRetiro,cliente")] Retiros retiros)
        {
            if (ModelState.IsValid)
            {
                // Asignar la fecha actual
                retiros.fechaRetiro = DateTime.Now;

                // Buscar el socio por su nombre
                if (!string.IsNullOrEmpty(retiros.cliente))
                {
                    var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == retiros.cliente);
                    if (clienteSeleccionado != null)
                    {
                        retiros.clienteID = clienteSeleccionado.clienteID; // Asigna el ID del socio seleccionado
                    }
                    else
                    {
                        ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                        ViewData["cliente"] = new SelectList(await _context.Retiros.ToListAsync(), "clienteID", "cliente");
                        return View(retiros);
                    }
                }

                _context.Add(retiros);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
            return View(retiros);
        }

        // GET: Retiros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retiros = await _context.Retiros.FindAsync(id);
            if (retiros == null)
            {
                return NotFound();
            }
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente", retiros.cliente);
            return View(retiros);
        }

        // POST: Retiros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("retiroID,cantidadRetiro,fechaRetiro,detalleRetiro,cliente")] Retiros retiros)
        {
            if (id != retiros.retiroID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar el socio por su nombre
                    if (!string.IsNullOrEmpty(retiros.cliente))
                    {
                        var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == retiros.cliente);
                        if (clienteSeleccionado != null)
                        {
                            retiros.clienteID = clienteSeleccionado.clienteID; // Asigna el ID del socio seleccionado
                        }
                        else
                        {
                            ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                            ViewData["cliente"] = new SelectList(await _context.Retiros.ToListAsync(), "clienteID", "cliente");
                            return View(retiros);
                        }
                    }

                    _context.Update(retiros);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RetirosExists(retiros.retiroID))
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
            return View(retiros);
        }

        // GET: Retiros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retiros = await _context.Retiros
                .FirstOrDefaultAsync(m => m.retiroID == id);
            if (retiros == null)
            {
                return NotFound();
            }

            return View(retiros);
        }

        // POST: Retiros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var retiros = await _context.Retiros.FindAsync(id);
            if (retiros != null)
            {
                _context.Retiros.Remove(retiros);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RetirosExists(int id)
        {
            return _context.Retiros.Any(e => e.retiroID == id);
        }
    }
}

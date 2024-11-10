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
    public class PasivosClientesController : Controller
    {
        private readonly CooperativaContext _context;

        public PasivosClientesController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: PasivosClientes
        public async Task<IActionResult> Index()
        {
            var CooperativaContext = _context.PasivosClientes.Include(p => p.clientes).Include(p => p.clientes);
            return View(await _context.PasivosClientes.ToListAsync());
        }

        // GET: PasivosClientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasivosClientes = await _context.PasivosClientes
                .Include(p => p.clientes)
                .FirstOrDefaultAsync(m => m.pasivoClienteID == id);
            if (pasivosClientes == null)
            {
                return NotFound();
            }

            return View(pasivosClientes);
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

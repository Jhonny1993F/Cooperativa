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
    public class TransferenciasController : Controller
    {
        private readonly CooperativaContext _context;

        public TransferenciasController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Transferencias
        public async Task<IActionResult> Index()
        {
            var CooperativaContext = _context.Transferencias.Include(p => p.clientes).Include(p => p.clientes);
            return View(await _context.Transferencias.ToListAsync());
        }

        // GET: Transferencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencias = await _context.Transferencias
                .Include(t => t.clientes)
                .FirstOrDefaultAsync(m => m.transferenciaID == id);
            if (transferencias == null)
            {
                return NotFound();
            }

            return View(transferencias);
        }

        // GET: Transferencias/Create
        public IActionResult Create()
        {
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente");
            return View();
        }

        // POST: Transferencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("transferenciaID,cantidadTransferencia,detalleTransferencia,cliente")] Transferencias transferencias)
        {
            if (ModelState.IsValid)
            {
                transferencias.fechaTransferencia = DateTime.Now;

                if (!string.IsNullOrEmpty(transferencias.cliente))
                {
                    var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == transferencias.cliente);
                    if (clienteSeleccionado != null)
                    {
                        transferencias.clienteID = clienteSeleccionado.clienteID;
                    }
                    else
                    {
                        ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                        ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
                        return View(transferencias);
                    }
                }

                _context.Add(transferencias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
            return View(transferencias);
        }

        // GET: Transferencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencias = await _context.Transferencias.FindAsync(id);
            if (transferencias == null)
            {
                return NotFound();
            }
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente", transferencias.cliente);
            return View(transferencias);
        }

        // POST: Transferencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("transferenciaID,cantidadTransferencia,fechaTransferencia,detalleTransferencia,cliente")] Transferencias transferencias)
        {
            if (id != transferencias.transferenciaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(transferencias.cliente))
                    {
                        var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == transferencias.cliente);
                        if (clienteSeleccionado != null)
                        {
                            transferencias.clienteID = clienteSeleccionado.clienteID;
                        }
                        else
                        {
                            ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
                            return View(transferencias);
                        }
                    }

                    _context.Update(transferencias);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferenciasExists(transferencias.transferenciaID))
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
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente", transferencias.clienteID);
            return View(transferencias);
        }

        // GET: Transferencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencias = await _context.Transferencias
                .FirstOrDefaultAsync(m => m.transferenciaID == id);
            if (transferencias == null)
            {
                return NotFound();
            }

            return View(transferencias);
        }

        // POST: Transferencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transferencias = await _context.Transferencias.FindAsync(id);
            if (transferencias != null)
            {
                _context.Transferencias.Remove(transferencias);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransferenciasExists(int id)
        {
            return _context.Transferencias.Any(e => e.transferenciaID == id);
        }
    }
}

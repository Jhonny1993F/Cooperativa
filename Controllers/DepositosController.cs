using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using System.ComponentModel;
using NuGet.Protocol.Plugins;

namespace Cooperativa.Controllers
{
    public class DepositosController : Controller
    {
        private readonly CooperativaContext _context;

        public DepositosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Depositos
        public async Task<IActionResult> Index()
        {
            var CooperativaContext = _context.Depositos.Include(c => c.clientes).Include(c => c.clientes);
            return View(await _context.Depositos.ToListAsync());
        }

        // GET: Depositos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositos = await _context.Depositos
                .Include(c => c.clientes)
                .FirstOrDefaultAsync(m => m.depositoID == id);
            if (depositos == null)
            {
                return NotFound();
            }

            return View(depositos);
        }

        // GET: Depositos/Create
        public IActionResult Create()
        {
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente");
            return View();
        }

        // POST: Depositos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("depositoID,cliente,cantidadDeposito,detalleDeposito")] Depositos depositos)
        {
            if (ModelState.IsValid)
            {
                // Asignar la fecha actual
                depositos.fechaDeposito = DateTime.Now;

                // Buscar el socio por su nombre
                if (!string.IsNullOrEmpty(depositos.cliente))
                {
                    var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == depositos.cliente);
                    if (clienteSeleccionado != null)
                    {
                        depositos.clienteID = clienteSeleccionado.clienteID; // Asigna el ID del socio seleccionado
                    }
                    else
                    {
                        ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                        ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
                        return View(depositos);
                    }
                }

                 // Agregar el registro a la base de datos
                 _context.Add(depositos);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
            }

            // En caso de error en el modelo, volver a cargar la lista de socios
            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
            return View(depositos);
        }

        // GET: Depositos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositos = await _context.Depositos.FindAsync(id);
            if (depositos == null)
            {
                return NotFound();
            }
            ViewData["cliente"] = new SelectList(_context.Set<Clientes>(), "cliente", "cliente", depositos.cliente);
            return View(depositos);
        }

        // POST: Depositos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("depositoID,cantidadDeposito,fechaDeposito,detalleDeposito,cliente")] Depositos depositos)
        {
            if (id != depositos.depositoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar el socio por su nombre
                    if (!string.IsNullOrEmpty(depositos.cliente))
                    {
                        var clienteSeleccionado = await _context.Clientes.FirstOrDefaultAsync(c => c.cliente == depositos.cliente);
                        if (clienteSeleccionado != null)
                        {
                            depositos.clienteID = clienteSeleccionado.clienteID; // Asigna el ID del socio seleccionado
                        }
                        else
                        {
                            ModelState.AddModelError("cliente", "El cliente seleccionado no existe.");
                            ViewData["cliente"] = new SelectList(await _context.Clientes.ToListAsync(), "clienteID", "cliente");
                            return View(depositos);
                        }
                    }

                    _context.Update(depositos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositosExists(depositos.depositoID))
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
            return View(depositos);
        }

        // GET: Depositos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositos = await _context.Depositos
                .FirstOrDefaultAsync(m => m.depositoID == id);
            if (depositos == null)
            {
                return NotFound();
            }

            return View(depositos);
        }

        // POST: Depositos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var depositos = await _context.Depositos.FindAsync(id);
            if (depositos != null)
            {
                _context.Depositos.Remove(depositos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepositosExists(int id)
        {
            return _context.Depositos.Any(e => e.depositoID == id);
        }
    }
}

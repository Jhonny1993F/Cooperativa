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
            var CooperativaContext = _context.Ahorros.Include(a => a.socios).Include(a => a.socios);
            return View(await _context.Ahorros.ToListAsync());
        }

        // GET: Ahorros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ahorros = await _context.Ahorros
                .Include(a => a.socios)
                .FirstOrDefaultAsync(m => m.ahorroID == id);
            if (ahorros == null)
            {
                return NotFound();
            }

            return View(ahorros);
        }

        // GET: Ahorros/Create
        public IActionResult Create()
        {
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio");
            return View();
        }

        // POST: Ahorros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ahorroID,montoAhorro,comprobante,detalleAhorro,socio")] Ahorros ahorros)
        {
            if (ModelState.IsValid)
            {
                // Asignar la fecha actual
                ahorros.fechaAhorro = DateTime.Now;

                // Buscar el socio por su nombre
                if (!string.IsNullOrEmpty(ahorros.socio))
                {
                    var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == ahorros.socio);
                    if (socioSeleccionado != null)
                    {
                        ahorros.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                    }
                    else
                    {
                        ModelState.AddModelError("socio", "El socio seleccionado no existe.");
                        ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                        return View(ahorros);
                    }
                }

                // Agregar el registro a la base de datos
                _context.Add(ahorros);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // En caso de error en el modelo, volver a cargar la lista de socios
            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
            return View(ahorros);
        }

        // GET: Ahorros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ahorros = await _context.Ahorros.FindAsync(id);
            if (ahorros == null)
            {
                return NotFound();
            }
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", ahorros.socio);
            return View(ahorros);
        }

        // POST: Ahorros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ahorroID,montoAhorro,comprobante,fechaAhorro,detalleAhorro,socio")] Ahorros ahorros)
        {
            if (id != ahorros.ahorroID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar el socio por su nombre
                    if (!string.IsNullOrEmpty(ahorros.socio))
                    {
                        var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == ahorros.socio);
                        if (socioSeleccionado != null)
                        {
                            ahorros.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                        }
                        else
                        {
                            ModelState.AddModelError("socio", "El socio seleccionado no existe.");
                            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                            return View(ahorros);
                        }
                    }
                    _context.Update(ahorros);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AhorrosExists(ahorros.ahorroID))
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
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", ahorros.socio);
            return View(ahorros);
        }

        // GET: Ahorros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ahorros = await _context.Ahorros
                .Include(a => a.socios)
                .FirstOrDefaultAsync(m => m.ahorroID == id);
            if (ahorros == null)
            {
                return NotFound();
            }

            return View(ahorros);
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
            return RedirectToAction(nameof(Index));
        }

        private bool AhorrosExists(int id)
        {
            return (_context.Ahorros?.Any(e => e.ahorroID == id)).GetValueOrDefault();
        }
    }
}

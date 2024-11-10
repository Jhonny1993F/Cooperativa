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
    public class CreditosController : Controller
    {
        private readonly CooperativaContext _context;

        public CreditosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Creditos
        public async Task<IActionResult> Index()
        {
            var CooperativaContext = _context.Creditos.Include(c => c.socios).Include(c => c.socios);
            return View(await _context.Creditos.ToListAsync());
        }

        // GET: Creditos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditos = await _context.Creditos
                .Include(c => c.socios)
                .FirstOrDefaultAsync(m => m.creditoID == id);
            if (creditos == null)
            {
                return NotFound();
            }

            return View(creditos);
        }

        // GET: Creditos/Create
        public IActionResult Create()
        {
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio");
            return View();
        }

        // POST: Creditos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("creditoID,montoCredito,tiempo,interes,cuota,estado,totalCredito,socio")] Creditos creditos)
        {
            if (ModelState.IsValid)
            {
                creditos.fechaCredito = DateTime.Now;
                //ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Buscar el socio por su nombre
                if (!string.IsNullOrEmpty(creditos.socio))
                {
                    var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == creditos.socio);
                    if (socioSeleccionado != null)
                    {
                        creditos.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                    }
                }

                _context.Add(creditos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", creditos.socio);
            return View(creditos);
        }

        // GET: Creditos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditos = await _context.Creditos.FindAsync(id);
            if (creditos == null)
            {
                return NotFound();
            }
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", creditos.socio);
            return View(creditos);
        }

        // POST: Creditos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("creditoID,montoCredito,fechaCredito,tiempo,interes,cuota,estado,totalCredito,socio")] Creditos creditos)
        {
            if (id != creditos.creditoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar el socio por su nombre
                    if (!string.IsNullOrEmpty(creditos.socio))
                    {
                        var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == creditos.socio);
                        if (socioSeleccionado != null)
                        {
                            creditos.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                        }
                    }

                    _context.Update(creditos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditosExists(creditos.creditoID))
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
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", creditos.socio);
            return View(creditos);
        }

        // GET: Creditos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditos = await _context.Creditos
                .Include(c => c.socios)
                .FirstOrDefaultAsync(m => m.creditoID == id);
            if (creditos == null)
            {
                return NotFound();
            }

            return View(creditos);
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
            return RedirectToAction(nameof(Index));
        }

        private bool CreditosExists(int id)
        {
            return _context.Creditos.Any(e => e.creditoID == id);
        }
    }
}

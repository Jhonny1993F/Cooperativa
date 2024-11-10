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
            var CooperativaContext = _context.Eventos.Include(a => a.socios).Include(a => a.socios);
            return View(await _context.Eventos.ToListAsync());
        }

        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventos = await _context.Eventos
                .Include(a => a.socios)
                .FirstOrDefaultAsync(m => m.eventoID == id);
            if (eventos == null)
            {
                return NotFound();
            }

            return View(eventos);
        }

        // GET: Eventos/Create
        public IActionResult Create()
        {
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio");
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
                        ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                        return View(eventos);
                    }
                }

                _context.Add(eventos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
            return View(eventos);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventos = await _context.Eventos.FindAsync(id);
            if (eventos == null)
            {
                return NotFound();
            }
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", eventos.socio);
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
                return RedirectToAction(nameof(Index));
            }
            return View(eventos);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventos = await _context.Eventos
                .FirstOrDefaultAsync(m => m.eventoID == id);
            if (eventos == null)
            {
                return NotFound();
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
            return RedirectToAction(nameof(Index));
        }

        private bool EventosExists(int id)
        {
            return _context.Eventos.Any(e => e.eventoID == id);
        }
    }
}

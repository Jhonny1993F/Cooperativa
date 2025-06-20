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
    public class SegurosController : Controller
    {
        private readonly CooperativaContext _context;

        public SegurosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Seguros
        public async Task<IActionResult> Index()
        {
            var cooperativaContext = _context.Seguros.Include(s => s.socios);
            return View(await cooperativaContext.ToListAsync());
        }

        // GET: Seguros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguros = await _context.Seguros
                .Include(s => s.socios)
                .FirstOrDefaultAsync(m => m.seguroID == id);
            if (seguros == null)
            {
                return NotFound();
            }

            return View(seguros);
        }

        // GET: Seguros/Create
        public IActionResult Create()
        {
            ViewData["socioID"] = new SelectList(_context.Socios, "socioID", "socio");
            return View();
        }

        // POST: Seguros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("seguroID,valor,fechaSeguro,Tipo,tiempo,descripcion,socioID")] Seguros seguros)
        {
            if (ModelState.IsValid)
            {
                seguros.inscripcion = seguros.valor * 0.05m; // Calcula inscripción como 5% del valor.
                seguros.socio = _context.Socios.FirstOrDefault(s => s.socioID == seguros.socioID)?.socio;
                _context.Add(seguros);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["socioID"] = new SelectList(_context.Socios, "socioID", "Nombre", seguros.socioID);
            return View(seguros);
        }

        public async Task<IActionResult> Reporte()
        {
            var reportes = await _context.Seguros
                .GroupBy(s => s.Tipo)
                .Select(g => new {
                    Tipo = g.Key,
                    TotalSeguros = g.Count(),
                    ValorPromedio = g.Average(s => s.valor)
                }).ToListAsync();

            return View(reportes);
        }

        // GET: Seguros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguros = await _context.Seguros.FindAsync(id);
            if (seguros == null)
            {
                return NotFound();
            }
            ViewData["socioID"] = new SelectList(_context.Socios, "socioID", "socioID", seguros.socioID);
            return View(seguros);
        }

        // POST: Seguros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("seguroID,valor,fechaSeguro,Tipo,tiempo,descripcion,socioID,socio,inscripcion")] Seguros seguros)
        {
            if (id != seguros.seguroID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seguros);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SegurosExists(seguros.seguroID))
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
            ViewData["socioID"] = new SelectList(_context.Socios, "socioID", "socioID", seguros.socioID);
            return View(seguros);
        }

        // GET: Seguros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguros = await _context.Seguros
                .Include(s => s.socios)
                .FirstOrDefaultAsync(m => m.seguroID == id);
            if (seguros == null)
            {
                return NotFound();
            }

            return View(seguros);
        }

        // POST: Seguros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seguros = await _context.Seguros.FindAsync(id);
            if (seguros != null)
            {
                _context.Seguros.Remove(seguros);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SegurosExists(int id)
        {
            return _context.Seguros.Any(e => e.seguroID == id);
        }
    }
}

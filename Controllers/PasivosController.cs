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
    public class PasivosController : Controller
    {
        private readonly CooperativaContext _context;

        public PasivosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Pasivos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pasivos.ToListAsync());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using Microsoft.IdentityModel.Tokens;

namespace Cooperativa.Controllers
{
    public class BancosController : Controller
    {
        private readonly CooperativaContext _context;

        public BancosController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Bancos
        public async Task<IActionResult> Index()
        {
            var cooperativaContext = _context.Bancos.Include(b => b.creditos);
            return View(await cooperativaContext.ToListAsync());
        }

        // GET: Bancos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bancos = await _context.Bancos
                .Include(b => b.creditos)
                .FirstOrDefaultAsync(m => m.BancoID == id);
            if (bancos == null)
            {
                return NotFound();
            }

            return View(bancos);
        }

        // GET: Bancos/Create
        public IActionResult Create()
        {
            ViewData["interes"] = new SelectList(_context.Creditos, "interes", "interes");
            return View();
        }

        // POST: Bancos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BancoID,nombre,interesBanco,cantidad,comparacion,interes")] Bancos bancos)
        {
            if (ModelState.IsValid)
            {
                if (bancos.interes > 0)
                {
                    var interesSeleccionado = await _context.Creditos.FirstOrDefaultAsync(b => b.interes == bancos.interes);
                    if (interesSeleccionado != null)
                    {
                        bancos.creditoID = interesSeleccionado.creditoID;
                    }
                    else
                    {
                        ModelState.AddModelError("banco", "El banco seleccionado no existe.");
                        ViewData["interes"] = new SelectList(await _context.Creditos.ToListAsync(), "interes", "interes");
                        return View(bancos);
                    }
                }

                _context.Add(bancos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["interes"] = new SelectList(_context.Creditos, "interes", "interes", bancos.interes);
            return View(bancos);
        }

        // GET: Bancos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bancos = await _context.Bancos.FindAsync(id);
            if (bancos == null)
            {
                return NotFound();
            }
            ViewData["interes"] = new SelectList(_context.Creditos, "interes", "interes", bancos.interes);
            return View(bancos);
        }

        // POST: Bancos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BancoID,nombre,interesBanco,cantidad,comparacion,creditoID,interes")] Bancos bancos)
        {
            if (id != bancos.BancoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (/*!string.IsNullOrEmpty(bancos.interes)*/ bancos.interes > 0)
                    {
                        var bancoSeleccionado = await _context.Creditos.FirstOrDefaultAsync(b => b.interes == bancos.interes);
                        if (bancoSeleccionado != null)
                        {
                            bancos.creditoID = bancoSeleccionado.creditoID;
                        }
                        else
                        {
                            ModelState.AddModelError("banco", "El banco seleccionado no existe.");
                            ViewData["interes"] = new SelectList(await _context.Creditos.ToListAsync(), "interes", "interes");
                            return View(bancos);
                        }
                    }

                    _context.Update(bancos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BancosExists(bancos.BancoID))
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
            ViewData["interes"] = new SelectList(_context.Creditos, "interes", "interes", bancos.interes);
            return View(bancos);
        }

        // GET: Bancos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bancos = await _context.Bancos
                .Include(b => b.creditos)
                .FirstOrDefaultAsync(m => m.BancoID == id);
            if (bancos == null)
            {
                return NotFound();
            }

            return View(bancos);
        }

        // POST: Bancos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bancos = await _context.Bancos.FindAsync(id);
            if (bancos != null)
            {
                _context.Bancos.Remove(bancos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BancosExists(int id)
        {
            return _context.Bancos.Any(e => e.BancoID == id);
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Cooperativa.Data;
//using Cooperativa.Models;

//namespace Cooperativa.Controllers
//{
//    public class UtilidadesController : Controller
//    {
//        private readonly CooperativaContext _context;

//        public UtilidadesController(CooperativaContext context)
//        {
//            _context = context;
//        }

//        // GET: Utilidades
//        public async Task<IActionResult> Index()
//        {
//            var utilidades = await _context.Utilidades
//                                           .Include(u => u.socios)
//                                           .Include(u => u.creditos)
//                                           .ToListAsync();
//            return View(utilidades);
//        }

//        // GET: Utilidades/Create
//        public IActionResult Create()
//        {
//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio");
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion");
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes");
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito");
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento");
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro");
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo");

//            return View();
//        }

//        // POST: Utilidades/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("utilidadID,utilidadTotal,utilidadPorSocio,interes,totalCredito,costoEvento,montoAhorro,costoPasivo,socioID,inscripcion")] Utilidades utilidades)
//        {
//            if (ModelState.IsValid)
//            {
//                // Validación y asignación de relaciones
//                var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socioID == utilidades.socioID);
//                var creditoSeleccionado = await _context.Creditos.FirstOrDefaultAsync(c => c.creditoID == utilidades.creditoID);

//                if (socioSeleccionado != null && creditoSeleccionado != null)
//                {
//                    utilidades.socioID = socioSeleccionado.socioID;
//                    utilidades.creditoID = creditoSeleccionado.creditoID;
//                    utilidades.fechaUtilidad = DateTime.Now;

//                    _context.Add(utilidades);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction(nameof(Index));
//                }
//                else
//                {
//                    ModelState.AddModelError("Error", "El socio o el crédito seleccionado no existe.");
//                }
//            }

//            // Recargar listas en caso de error
//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio", utilidades.socioID);
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion", utilidades.inscripcion);
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes", utilidades.interes);
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito", utilidades.totalCredito);
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento", utilidades.costoEvento);
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro", utilidades.montoAhorro);
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo", utilidades.costoPasivo);

//            return View(utilidades);
//        }

//        // GET: Utilidades/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null) return NotFound();

//            var utilidades = await _context.Utilidades.FindAsync(id);
//            if (utilidades == null) return NotFound();

//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio", utilidades.socioID);
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion", utilidades.inscripcion);
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes", utilidades.interes);
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito", utilidades.totalCredito);
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento", utilidades.costoEvento);
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro", utilidades.montoAhorro);
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo", utilidades.costoPasivo);

//            return View(utilidades);
//        }

//        // POST: Utilidades/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("utilidadID,utilidadTotal,utilidadPorSocio,interes,totalCredito,costoEvento,montoAhorro,costoPasivo,socioID,inscripcion")] Utilidades utilidades)
//        {
//            if (id != utilidades.utilidadID) return NotFound();

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socioID == utilidades.socioID);
//                    var creditoSeleccionado = await _context.Creditos.FirstOrDefaultAsync(c => c.creditoID == utilidades.creditoID);

//                    if (socioSeleccionado != null && creditoSeleccionado != null)
//                    {
//                        utilidades.socioID = socioSeleccionado.socioID;
//                        utilidades.creditoID = creditoSeleccionado.creditoID;

//                        _context.Update(utilidades);
//                        await _context.SaveChangesAsync();
//                        return RedirectToAction(nameof(Index));
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("Error", "El socio o el crédito seleccionado no existe.");
//                    }
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!UtilidadesExists(utilidades.utilidadID)) return NotFound();
//                    throw;
//                }
//            }

//            // Recargar listas en caso de error
//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio", utilidades.socioID);
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion", utilidades.inscripcion);
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes", utilidades.interes);
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito", utilidades.totalCredito);
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento", utilidades.costoEvento);
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro", utilidades.montoAhorro);
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo", utilidades.costoPasivo);

//            return View(utilidades);
//        }

//        // GET: Utilidades/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null) return NotFound();

//            var utilidades = await _context.Utilidades
//                                           .Include(u => u.socios)
//                                           .Include(u => u.creditos)
//                                           .FirstOrDefaultAsync(m => m.utilidadID == id);
//            if (utilidades == null) return NotFound();

//            return View(utilidades);
//        }

//        // POST: Utilidades/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var utilidades = await _context.Utilidades.FindAsync(id);
//            _context.Utilidades.Remove(utilidades);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool UtilidadesExists(int id)
//        {
//            return _context.Utilidades.Any(e => e.utilidadID == id);
//        }
//    }
//}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Threading.Tasks;
//using Cooperativa.Data;
//using Cooperativa.Models;

//namespace Cooperativa.Controllers
//{
//    public class UtilidadesController : Controller
//    {
//        private readonly CooperativaContext _context;

//        public UtilidadesController(CooperativaContext context)
//        {
//            _context = context;
//        }

//        // GET: Utilidades
//        public async Task<IActionResult> Index()
//        {
//            var utilidades = await _context.Utilidades
//                                           .Include(u => u.socios)
//                                           .Include(u => u.creditos)
//                                           .ToListAsync();
//            return View(utilidades);
//        }

//        // GET: Utilidades/Create
//        public IActionResult Create()
//        {
//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio");
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion");
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes");
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito");
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento");
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro");
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo");

//            return View();
//        }

//        // POST: Utilidades/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("utilidadID,utilidadTotal,utilidadPorSocio,interes,totalCredito,costoEvento,montoAhorro,costoPasivo,socioID,inscripcion")] Utilidades utilidades)
//        {
//            if (ModelState.IsValid)
//            {
//                // Asignar la fecha actual de la utilidad
//                utilidades.fechaUtilidad = DateTime.Now;

//                // Validación y asignación automática del socioID y creditoID
//                var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socioID == utilidades.socioID);
//                var creditoSeleccionado = await _context.Creditos.FirstOrDefaultAsync(c => c.socioID == utilidades.socioID);

//                if (socioSeleccionado != null && creditoSeleccionado != null)
//                {
//                    utilidades.socioID = socioSeleccionado.socioID;
//                    utilidades.creditoID = creditoSeleccionado.creditoID;

//                    _context.Add(utilidades);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction(nameof(Index));
//                }
//                else
//                {
//                    ModelState.AddModelError("Error", "El socio o el crédito seleccionado no existe.");
//                }
//            }

//            // Si hay un error, recargar los valores en los dropdowns
//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio", utilidades.socioID);
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion", utilidades.inscripcion);
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes", utilidades.interes);
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito", utilidades.totalCredito);
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento", utilidades.costoEvento);
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro", utilidades.montoAhorro);
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo", utilidades.costoPasivo);

//            return View(utilidades);
//        }

//        // GET: Utilidades/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//                return NotFound();

//            var utilidades = await _context.Utilidades.FindAsync(id);
//            if (utilidades == null)
//                return NotFound();

//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio", utilidades.socioID);
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion", utilidades.inscripcion);
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes", utilidades.interes);
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito", utilidades.totalCredito);
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento", utilidades.costoEvento);
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro", utilidades.montoAhorro);
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo", utilidades.costoPasivo);

//            return View(utilidades);
//        }

//        // POST: Utilidades/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("utilidadID,utilidadTotal,utilidadPorSocio,interes,totalCredito,costoEvento,montoAhorro,costoPasivo,socioID,inscripcion")] Utilidades utilidades)
//        {
//            if (id != utilidades.utilidadID)
//                return NotFound();

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socioID == utilidades.socioID);
//                    var creditoSeleccionado = await _context.Creditos.FirstOrDefaultAsync(c => c.socioID == utilidades.socioID);

//                    if (socioSeleccionado != null && creditoSeleccionado != null)
//                    {
//                        utilidades.socioID = socioSeleccionado.socioID;
//                        utilidades.creditoID = creditoSeleccionado.creditoID;

//                        _context.Update(utilidades);
//                        await _context.SaveChangesAsync();
//                        return RedirectToAction(nameof(Index));
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("Error", "El socio o el crédito seleccionado no existe.");
//                    }
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!UtilidadesExists(utilidades.utilidadID))
//                        return NotFound();
//                    throw;
//                }
//            }

//            // Recargar listas en caso de error
//            ViewData["socio"] = new SelectList(_context.Socios, "socioID", "socio", utilidades.socioID);
//            ViewData["inscripcion"] = new SelectList(_context.Socios, "socioID", "inscripcion", utilidades.inscripcion);
//            ViewData["interes"] = new SelectList(_context.Creditos, "creditoID", "interes", utilidades.interes);
//            ViewData["totalCredito"] = new SelectList(_context.Creditos, "creditoID", "totalCredito", utilidades.totalCredito);
//            ViewData["costoEvento"] = new SelectList(_context.Creditos, "creditoID", "costoEvento", utilidades.costoEvento);
//            ViewData["montoAhorro"] = new SelectList(_context.Creditos, "creditoID", "montoAhorro", utilidades.montoAhorro);
//            ViewData["costoPasivo"] = new SelectList(_context.Creditos, "creditoID", "costoPasivo", utilidades.costoPasivo);

//            return View(utilidades);
//        }

//        // GET: Utilidades/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//                return NotFound();

//            var utilidades = await _context.Utilidades
//                                           .Include(u => u.socios)
//                                           .Include(u => u.creditos)
//                                           .FirstOrDefaultAsync(m => m.utilidadID == id);
//            if (utilidades == null)
//                return NotFound();

//            return View(utilidades);
//        }

//        // POST: Utilidades/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var utilidades = await _context.Utilidades.FindAsync(id);
//            _context.Utilidades.Remove(utilidades);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool UtilidadesExists(int id)
//        {
//            return _context.Utilidades.Any(e => e.utilidadID == id);
//        }
//    }
//}


using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;

namespace Cooperativa.Controllers
{
    public class UtilidadesController : Controller
    {
        private readonly CooperativaContext _context;

        public UtilidadesController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Utilidades
        public async Task<IActionResult> Index()
        {
            var cooperativaContext = _context.Utilidades.Include(u => u.socios)
                                                         .Include(u => u.creditos)
                                                         .Include(u => u.eventos)
                                                         .Include(u => u.ahorros)
                                                         .Include(u => u.pasivos);
            return View(await cooperativaContext.ToListAsync());
        }

        // GET: Utilidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilidades = await _context.Utilidades
                .Include(u => u.socios)
                .Include(u => u.creditos)
                .Include(u => u.eventos)
                .Include(u => u.ahorros)
                .Include(u => u.pasivos)
                .FirstOrDefaultAsync(m => m.utilidadID == id);
            if (utilidades == null)
            {
                return NotFound();
            }

            return View(utilidades);
        }

        // GET: Utilidades/Create
        public IActionResult Create()
        {
            ViewData["socio"] = new SelectList(_context.Socios, "socio", "socio");
            ViewData["inscripcion"] = new SelectList(_context.Socios, "inscripcion", "inscripcion");
            ViewData["interes"] = new SelectList(_context.Creditos, "interes", "interes");
            ViewData["totalCredito"] = new SelectList(_context.Creditos, "totalCredito", "totalCredito");
            ViewData["costoEvento"] = new SelectList(_context.Eventos, "costoEvento", "costoEvento");
            ViewData["montoAhorro"] = new SelectList(_context.Ahorros, "montoAhorro", "montoAhorro");
            ViewData["costoPasivo"] = new SelectList(_context.Pasivos, "costoPasivo", "costoPasivo");
            return View();
        }

        // POST: Utilidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("utilidadID,utilidadTotal,utilidadPorSocio,socio,inscripcion,interes,totalCredito,costoEvento,montoAhorro,costoPasivo")] Utilidades utilidades)
        {
            if (ModelState.IsValid)
            {
                utilidades.fechaUtilidad = DateTime.Now;

                // Buscar el socio por su nombre
                if (!string.IsNullOrEmpty(utilidades.socio))
                {
                    var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == utilidades.socio);
                    if (socioSeleccionado != null)
                    {
                        utilidades.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                    }
                    else
                    {
                        ModelState.AddModelError("socio", "El socio seleccionado no existe.");
                        ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                        return View(utilidades);
                    }
                }

                if (utilidades.interes > 0)
                {
                    var interesSeleccionado = await _context.Creditos.FirstOrDefaultAsync(b => b.interes == utilidades.interes);
                    if (interesSeleccionado != null)
                    {
                        utilidades.creditoID = interesSeleccionado.creditoID;
                    }
                    else
                    {
                        ModelState.AddModelError("utilidad", "La utilidad seleccionada no existe.");
                        ViewData["interes"] = new SelectList(await _context.Creditos.ToListAsync(), "CreditoID", "interes");
                        return View(utilidades);
                    }
                }

                if (utilidades.costoEvento > 0)
                {
                    var costoEventoSeleccionado = await _context.Eventos.FirstOrDefaultAsync(u => u.costoEvento == utilidades.costoEvento);
                    if (costoEventoSeleccionado != null)
                    {
                        utilidades.eventoID = costoEventoSeleccionado.eventoID;
                    }
                    else
                    {
                        ModelState.AddModelError("evento", "El evento seleccionado no existe.");
                        ViewData["costoEvento"] = new SelectList(await _context.Eventos.ToListAsync(), "eventoID", "costoEvento");
                        return View(utilidades);
                    }
                }

                if (utilidades.montoAhorro > 0)
                {
                    var montoAhorroSeleccionado = await _context.Ahorros.FirstOrDefaultAsync(u => u.montoAhorro == utilidades.montoAhorro);
                    if (montoAhorroSeleccionado != null)
                    {
                        utilidades.ahorroID = montoAhorroSeleccionado.ahorroID;
                    }
                    else
                    {
                        ModelState.AddModelError("ahorro", "El ahorro seleccionado no existe.");
                        ViewData["montoAhorro"] = new SelectList(await _context.Ahorros.ToListAsync(), "ahorroID", "montoAhorro");
                        return View(utilidades);
                    }
                }

                if (utilidades.costoPasivo > 0)
                {
                    var costoPasivoSeleccionado = await _context.Pasivos.FirstOrDefaultAsync(u => u.costoPasivo == utilidades.costoPasivo);
                    if (costoPasivoSeleccionado != null)
                    {
                        utilidades.pasivoID = costoPasivoSeleccionado.pasivoID;
                    }
                    else
                    {
                        ModelState.AddModelError("pasivo", "El pasivo seleccionado no existe.");
                        ViewData["costoPasivo"] = new SelectList(await _context.Pasivos.ToListAsync(), "pasivoID", "costoPasivo");
                        return View(utilidades);
                    }
                }

                _context.Add(utilidades);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socio", "socio");
            ViewData["inscripcion"] = new SelectList(await _context.Socios.ToListAsync(), "inscripcion", "inscripcion");
            ViewData["interes"] = new SelectList(await _context.Creditos.ToListAsync(), "interes", "interes");
            ViewData["totalCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "totalCredito", "totalCredito");
            ViewData["costoEvento"] = new SelectList(await _context.Eventos.ToListAsync(), "costoEvento", "costoEvento");
            ViewData["montoAhorro"] = new SelectList(await _context.Ahorros.ToListAsync(), "montoAhorro", "montoAhorro");
            ViewData["costoPasivo"] = new SelectList(await _context.Pasivos.ToListAsync(), "costoPasivo", "costoPasivo");
            return View(utilidades);
        }

        // GET: Utilidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilidades = await _context.Utilidades.FindAsync(id);
            if (utilidades == null)
            {
                return NotFound();
            }
            ViewData["socio"] = new SelectList(_context.Set<Socios>(), "socio", "socio", utilidades.socio);
            ViewData["inscripcion"] = new SelectList(_context.Set<Socios>(), "inscripcion", "inscripcion", utilidades.inscripcion);
            ViewData["interes"] = new SelectList(_context.Set<Creditos>(), "interes", "interes", utilidades.interes);
            ViewData["totalCredito"] = new SelectList(_context.Set<Creditos>(), "totalCredito", "totalCredito", utilidades.totalCredito);
            ViewData["costoEvento"] = new SelectList(_context.Set<Eventos>(), "costoEvento", "costoEvento", utilidades.costoEvento);
            ViewData["montoAhorro"] = new SelectList(_context.Set<Ahorros>(), "montoAhorro", "montoAhorro", utilidades.montoAhorro);
            ViewData["costoPasivo"] = new SelectList(_context.Set<Pasivos>(), "costoPasivo", "costoPasivo", utilidades.costoPasivo);
            return View(utilidades);
        }

        // POST: Utilidades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("utilidadID,socio,utilidadTotal,utilidadPorSocio,fechaUtilidad,inscripcion,interes,totalCredito,costoEvento,montoAhorro,costoPasivo")] Utilidades utilidades)
        {
            if (id != utilidades.utilidadID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar el socio por su nombre
                    if (!string.IsNullOrEmpty(utilidades.socio))
                    {
                        var socioSeleccionado = await _context.Socios.FirstOrDefaultAsync(s => s.socio == utilidades.socio);
                        if (socioSeleccionado != null)
                        {
                            utilidades.socioID = socioSeleccionado.socioID; // Asigna el ID del socio seleccionado
                        }
                        else
                        {
                            ModelState.AddModelError("socio", "El socio seleccionado no existe.");
                            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socioID", "socio");
                            return View(utilidades);
                        }
                    }

                    if (utilidades.interes > 0)
                    {
                        var interesSeleccionado = await _context.Creditos.FirstOrDefaultAsync(b => b.interes == utilidades.interes);
                        if (interesSeleccionado != null)
                        {
                            utilidades.creditoID = interesSeleccionado.creditoID;
                        }
                        else
                        {
                            ModelState.AddModelError("utilidad", "La utilidad seleccionada no existe.");
                            ViewData["interes"] = new SelectList(await _context.Creditos.ToListAsync(), "interes", "interes");
                            return View(utilidades);
                        }
                    }

                    if (utilidades.costoEvento > 0)
                    {
                        var costoEventoSeleccionado = await _context.Eventos.FirstOrDefaultAsync(u => u.costoEvento == utilidades.costoEvento);
                        if (costoEventoSeleccionado != null)
                        {
                            utilidades.eventoID = costoEventoSeleccionado.eventoID;
                        }
                        else
                        {
                            ModelState.AddModelError("evento", "El evento seleccionado no existe.");
                            ViewData["costoEvento"] = new SelectList(await _context.Eventos.ToListAsync(), "costoEvento", "costoEvento");
                            return View(utilidades);
                        }
                    }

                    if (utilidades.montoAhorro > 0)
                    {
                        var montoAhorroSeleccionado = await _context.Ahorros.FirstOrDefaultAsync(u => u.montoAhorro == utilidades.montoAhorro);
                        if (montoAhorroSeleccionado != null)
                        {
                            utilidades.ahorroID = montoAhorroSeleccionado.ahorroID;
                        }
                        else
                        {
                            ModelState.AddModelError("ahorro", "El ahorro seleccionado no existe.");
                            ViewData["montoAhorro"] = new SelectList(await _context.Ahorros.ToListAsync(), "montoAhorro", "montoAhorro");
                            return View(utilidades);
                        }
                    }

                    if (utilidades.costoPasivo > 0)
                    {
                        var costoPasivoSeleccionado = await _context.Pasivos.FirstOrDefaultAsync(u => u.costoPasivo == utilidades.costoPasivo);
                        if (costoPasivoSeleccionado != null)
                        {
                            utilidades.pasivoID = costoPasivoSeleccionado.pasivoID;
                        }
                        else
                        {
                            ModelState.AddModelError("pasivo", "El pasivo seleccionado no existe.");
                            ViewData["costoPasivo"] = new SelectList(await _context.Pasivos.ToListAsync(), "costoPasivo", "costoPasivo");
                            return View(utilidades);
                        }
                    }

                    _context.Update(utilidades);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilidadesExists(utilidades.utilidadID))
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
            ViewData["socio"] = new SelectList(await _context.Socios.ToListAsync(), "socio", "socio", utilidades.socio);
            ViewData["inscripcion"] = new SelectList(await _context.Socios.ToListAsync(), "inscripcion", "inscripcion", utilidades.inscripcion);
            ViewData["interes"] = new SelectList(await _context.Creditos.ToListAsync(), "interes", "interes", utilidades.interes);
            ViewData["totalCredito"] = new SelectList(await _context.Creditos.ToListAsync(), "totalCredito", "totalCredito", utilidades.totalCredito);
            ViewData["costoEvento"] = new SelectList(await _context.Eventos.ToListAsync(), "costoEvento", "costoEvento", utilidades.costoEvento);
            ViewData["montoAhorro"] = new SelectList(await _context.Ahorros.ToListAsync(), "montoAhorro", "montoAhorro", utilidades.montoAhorro);
            ViewData["costoPasivo"] = new SelectList(await _context.Pasivos.ToListAsync(), "costoPasivo", "costoPasivo", utilidades.costoPasivo);
            return View(utilidades);
        }

        // GET: Utilidades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilidades = await _context.Utilidades
                .Include(u => u.socios)
                .Include(u => u.creditos)
                .Include(u => u.eventos)
                .Include(u => u.ahorros)
                .Include(u => u.pasivos)
                .FirstOrDefaultAsync(m => m.utilidadID == id);
            if (utilidades == null)
            {
                return NotFound();
            }

            return View(utilidades);
        }

        // POST: Utilidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilidades = await _context.Utilidades.FindAsync(id);
            if (utilidades != null)
            {
                _context.Utilidades.Remove(utilidades);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilidadesExists(int id)
        {
            return _context.Utilidades.Any(e => e.utilidadID == id);
        }
    }
}

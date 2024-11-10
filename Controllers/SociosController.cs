using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cooperativa.Controllers
{
    //[Authorize]
    public class SociosController : Controller
    {
        private readonly CooperativaContext _context;

        public SociosController(CooperativaContext context)
        {
            _context = context;
        }

        //// GET: Socios
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Socios.ToListAsync());
        //}

        // GET: Socios

        public async Task<IActionResult> Index()
        {
            try
            {
                var socios = await _context.Socios.ToListAsync();

                // Si es una petición AJAX o JSON, devuelve el JSON
                if (Request.Headers["Accept"] == "application/json")
                {
                    return Json(socios);
                }

                // Si no, devuelve la vista tradicional
                return View(socios);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, new { message = "Error al obtener socios: " + ex.Message });
            }
        }

        //// GET: Socios/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var socios = await _context.Socios
        //        .FirstOrDefaultAsync(m => m.socioID == id);
        //    if (socios == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(socios);
        //}

        // GET: Socios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socios = await _context.Socios.FirstOrDefaultAsync(m => m.socioID == id);
            if (socios == null)
            {
                return NotFound();
            }

            // Si es una petición JSON, devuelve el JSON
            if (Request.Headers["Accept"] == "application/json")
            {
                return Json(socios);
            }

            // Si no, devuelve la vista tradicional
            return View(socios);
        }

        // GET: Socios/Create
        public IActionResult Create()
        {
            return View();
        }

        //// POST: Socios/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("socioID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,socio,inscripcion,correo,contraseña")] Socios socios)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(socios);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(socios);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("socioID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,socio,inscripcion,correo,contraseña")] Socios socios)
        {
            if (ModelState.IsValid)
            {
                _context.Add(socios);
                await _context.SaveChangesAsync();

                // Si la petición es JSON, devuelve el objeto creado en formato JSON
                if (Request.Headers["Accept"] == "application/json")
                {
                    return Json(socios);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(socios);
        }

        // GET: Socios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socios = await _context.Socios.FindAsync(id);
            if (socios == null)
            {
                return NotFound();
            }
            return View(socios);
        }

        //// POST: Socios/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("socioID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,socio,inscripcion,correo,contraseña")] Socios socios)
        //{
        //    if (id != socios.socioID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(socios);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!SociosExists(socios.socioID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(socios);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("socioID,nombres,apellidos,cedula,fechaNacimiento,direccion,telefono,socio,inscripcion,correo,contraseña")] Socios socios)
        {
            if (id != socios.socioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(socios);
                    await _context.SaveChangesAsync();

                    // Si es una petición JSON, devuelve el objeto actualizado
                    if (Request.Headers["Accept"] == "application/json")
                    {
                        return Json(socios);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SociosExists(socios.socioID))
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

            return View(socios);
        }

        // GET: Socios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socios = await _context.Socios
                .FirstOrDefaultAsync(m => m.socioID == id);
            if (socios == null)
            {
                return NotFound();
            }

            return View(socios);
        }

        //// POST: Socios/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var socios = await _context.Socios.FindAsync(id);
        //    if (socios != null)
        //    {
        //        _context.Socios.Remove(socios);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool SociosExists(int id)
        {
            return _context.Socios.Any(e => e.socioID == id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var socios = await _context.Socios.FindAsync(id);
            if (socios != null)
            {
                _context.Socios.Remove(socios);
                await _context.SaveChangesAsync();

                // Si la petición es JSON, devuelve un estado o un objeto vacío
                if (Request.Headers["Accept"] == "application/json")
                {
                    return Json(new { success = true });
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

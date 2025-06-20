using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooperativa.Data;
using Cooperativa.Models;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Cooperativa.Controllers
{
    public class UtilidadesController : Controller
    {
        private readonly CooperativaContext _context;

        public UtilidadesController(CooperativaContext context)
        {
            _context = context;
        }

        // GET: Socios
        public async Task<IActionResult> Index()
        {
            // Obtener el ID y el tipo de usuario (Socio o Cliente) desde los claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst("TipoUsuario")?.Value; // Obtienes el "TipoUsuario" (Socio o Cliente)

            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["ErrorMessage"] = "Usuario no válido";
                return RedirectToAction("Index");
            }
            // Si es socio administrador (ID = 2)
            if (userRoleClaim == "Socio" && userId == 2)
            {
                // El administrador ve todos los ahorros
                var utilidades = await _context.Utilidades.ToListAsync();
                return View(utilidades);
            }

            // Si es un socio normal
            if (userRoleClaim == "Socio")
            {
                // Filtrar los ahorros por socioID
                var utilidad = await _context.Utilidades
                    .Where(a => a.socioID == userId)
                    .ToListAsync();
                return View(utilidad);
            }

            /*// Si es un cliente
            if (userRoleClaim == "Cliente")
            {
                // Filtrar los ahorros por clienteID
                var utilidad = await _context.Utilidades
                    .Where(a => a.clienteID == userId)
                    .ToListAsync();
                return View(utilidad);
            }*/

            // Si el usuario no es socio, cerrar la sesión y redirigir al Home
            await HttpContext.SignOutAsync(); // Esto cierra la sesión
            TempData["ErrorMessage"] = "No tienes permisos para esta acción";
            return RedirectToAction("Index", "Home"); // Redirigir al Home
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
        public async Task<IActionResult> Create()
        {
            ViewBag.socio = new SelectList(await _context.Socios.ToListAsync(), "socio", "socio");
            return View(new Utilidades());
        }

        // POST: Utilidades/Create
        [HttpPost]
        public async Task<IActionResult> Create(Utilidades utilidades)
        {
            // Verificar si el modelo es válido
            if (ModelState.IsValid)
            {
                // Verificar si el socio seleccionado existe
                var socioID = await _context.Socios.FirstOrDefaultAsync(sID => sID.socioID == utilidades.socioID);
                var socio = await _context.Socios.FirstOrDefaultAsync(s => s.socio == utilidades.socio);
                var credito = await _context.Creditos.FirstOrDefaultAsync(c => c.creditoID == c.creditoID);
                var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.eventoID == e.eventoID);
                var ahorro = await _context.Ahorros.FirstOrDefaultAsync(a => a.ahorroID == a.ahorroID);
                var pasivo = await _context.Pasivos.FirstOrDefaultAsync(p => p.pasivoID == p.pasivoID);

                if (socio == null)
                {
                    // Si no se encuentra el socio, agregar error de modelo
                    ModelState.AddModelError("", "El socio seleccionado no existe.");
                    ViewBag.socio = new SelectList(_context.Socios, "socio", "socio");
                    return View(utilidades);
                }

                // Realizar cálculos basados en el socio seleccionado
                var totalAhorro = _context.Ahorros.Where(a => a.socioID == socio.socioID).Sum(a => (decimal?)a.montoAhorro) ?? 0;
                var ahorroTotal = _context.Ahorros.Sum(a => (decimal?)a.montoAhorro) ?? 0;
                var totalInteres = _context.Creditos.Sum(i => (decimal?)i.interes) ?? 0;
                var totalCredito = _context.Creditos.Sum(c => (decimal?)c.totalCredito) ?? 0;
                var totalEventos = _context.Eventos.Where(e => e.socioID == socio.socioID).Sum(e => (decimal?)e.costoEvento) ?? 0;
                var eventosTotal = _context.Eventos.Sum(e => (decimal?)e.costoEvento) ?? 0;
                var costoPasivo = _context.Pasivos.Sum(p => (decimal?)p.costoPasivo) ?? 0;

                // Dividir el costo del pasivo entre el número total de socios
                var totalSocios = _context.Socios.Count();
                costoPasivo /= totalSocios;

                // Realizar los cálculos de utilidad
                var utilidadTotal = ahorroTotal + totalCredito + eventosTotal - costoPasivo;
                var utilidadPorSocio = (totalEventos + totalAhorro) + ((totalAhorro / ahorroTotal) * totalInteres) - costoPasivo;

                // Asignar los valores calculados al modelo para que se muestren en el formulario
                utilidades.utilidadTotal = utilidadTotal;
                utilidades.utilidadPorSocio = utilidadPorSocio;

                //Asignar la fecha actual
                utilidades.fechaUtilidad = DateTime.Now;

                utilidades.inscripcion = socio.inscripcion;
                utilidades.interes = totalInteres;
                utilidades.totalCredito = totalCredito;
                utilidades.montoAhorro = totalAhorro;
                utilidades.costoEvento = totalEventos;
                utilidades.costoPasivo = costoPasivo;

                //Asignar los valores al modelo para guardarlos
                utilidades.socioID = socio.socioID;

                if (credito != null)
                {
                    utilidades.creditoID = credito.creditoID;
                }

                if (evento != null)
                {
                    utilidades.eventoID = evento.eventoID;
                }

                if (ahorro != null)
                {
                    utilidades.ahorroID = ahorro.ahorroID;
                }

                if (pasivo != null)
                {
                    utilidades.pasivoID = pasivo.pasivoID;
                }


                // Guardar los datos de la utilidad en la base de datos si es necesario
                _context.Utilidades.Add(utilidades);
                _context.SaveChanges();

                // Volver a cargar la lista de socios en el ViewBag para el dropdown
                ViewBag.socio = new SelectList(_context.Socios, "socio", "socio");

                // Redirigir a la vista de detalles o lista después de guardar
                return RedirectToAction(nameof(Index)); // Puedes redirigir a la vista que prefieras
            }

            // Si el modelo no es válido, cargar el select de socios y volver a mostrar el formulario
            ViewBag.socio = new SelectList(_context.Socios, "socio", "socio");
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
            return View(utilidades);
        }

        // POST: Utilidades/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Utilidades utilidades)
        {
            // Verificar si el modelo es válido
            if (ModelState.IsValid)
            {
                // Verificar si el socio seleccionado existe
                var socioID = await _context.Socios.FirstOrDefaultAsync(sID => sID.socioID == utilidades.socioID);
                var socio = await _context.Socios.FirstOrDefaultAsync(s => s.socio == utilidades.socio);
                var credito = await _context.Creditos.FirstOrDefaultAsync(c => c.creditoID == c.creditoID);
                var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.eventoID == e.eventoID);
                var ahorro = await _context.Ahorros.FirstOrDefaultAsync(a => a.ahorroID == a.ahorroID);
                var pasivo = await _context.Pasivos.FirstOrDefaultAsync(p => p.pasivoID == p.pasivoID);

                if (socio == null)
                {
                    // Si no se encuentra el socio, agregar error de modelo
                    ModelState.AddModelError("", "El socio seleccionado no existe.");
                    ViewBag.socio = new SelectList(_context.Socios, "socio", "socio");
                    return View(utilidades);
                }

                // Realizar cálculos basados en el socio seleccionado
                var totalAhorro = _context.Ahorros.Where(a => a.socioID == socio.socioID).Sum(a => (decimal?)a.montoAhorro) ?? 0;
                var ahorroTotal = _context.Ahorros.Sum(a => (decimal?)a.montoAhorro) ?? 0;
                var totalInteres = _context.Creditos.Sum(i => (decimal?)i.interes) ?? 0;
                var totalCredito = _context.Creditos.Sum(c => (decimal?)c.totalCredito) ?? 0;
                var totalEventos = _context.Eventos.Where(e => e.socioID == socio.socioID).Sum(e => (decimal?)e.costoEvento) ?? 0;
                var eventosTotal = _context.Eventos.Sum(e => (decimal?)e.costoEvento) ?? 0;
                var costoPasivo = _context.Pasivos.Sum(p => (decimal?)p.costoPasivo) ?? 0;

                // Dividir el costo del pasivo entre el número total de socios
                var totalSocios = _context.Socios.Count();
                costoPasivo /= totalSocios;

                // Realizar los cálculos de utilidad
                var utilidadTotal = ahorroTotal + totalCredito + eventosTotal - costoPasivo;
                var utilidadPorSocio = (totalEventos + totalAhorro) + ((totalAhorro / ahorroTotal) * totalInteres) - costoPasivo;

                // Asignar los valores calculados al modelo para que se muestren en el formulario
                utilidades.utilidadTotal = utilidadTotal;
                utilidades.utilidadPorSocio = utilidadPorSocio;

                //Asignar la fecha actual
                utilidades.fechaUtilidad = DateTime.Now;

                utilidades.inscripcion = socio.inscripcion;
                utilidades.interes = totalInteres;
                utilidades.totalCredito = totalCredito;
                utilidades.montoAhorro = totalAhorro;
                utilidades.costoEvento = totalEventos;
                utilidades.costoPasivo = costoPasivo;

                //Asignar los valores al modelo para guardarlos
                utilidades.socioID = socio.socioID;

                if (credito != null)
                {
                    utilidades.creditoID = credito.creditoID;
                }

                if (evento != null)
                {
                    utilidades.eventoID = evento.eventoID;
                }

                if (ahorro != null)
                {
                    utilidades.ahorroID = ahorro.ahorroID;
                }

                if (pasivo != null)
                {
                    utilidades.pasivoID = pasivo.pasivoID;
                }


                // Guardar los datos de la utilidad en la base de datos si es necesario
                _context.Utilidades.Update(utilidades);
                _context.SaveChanges();

                // Volver a cargar la lista de socios en el ViewBag para el dropdown
                ViewBag.socio = new SelectList(_context.Socios, "socio", "socio");

                // Redirigir a la vista de detalles o lista después de guardar
                return RedirectToAction(nameof(Index)); // Puedes redirigir a la vista que prefieras
            }

            // Si el modelo no es válido, cargar el select de socios y volver a mostrar el formulario
            ViewBag.socio = new SelectList(_context.Socios, "socio", "socio");
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


        //// Método para obtener los socios con más utilidades en un rango de fechas
        //public async Task<IActionResult> GetTopSociosByUtilidades(DateTime startDate, DateTime endDate)
        //{
        //    var topSocios = await _context.Utilidades
        //        .Where(u => u.fechaUtilidad >= startDate && u.fechaUtilidad <= endDate)
        //        .GroupBy(u => u.socioID)
        //        .Select(g => new
        //        {
        //            SocioID = g.Key,
        //            NombreSocio = g.FirstOrDefault().socio, // Asumiendo que 'socio' tiene el nombre del socio
        //            TotalUtilidad = g.Sum(u => u.utilidadPorSocio)
        //        })
        //        .OrderByDescending(s => s.TotalUtilidad)
        //        .ToListAsync();

        //    // Puedes devolver estos datos a una vista o simplemente redirigir con los resultados
        //    ViewBag.TopSocios = topSocios;
        //    return View(); // Crear una vista para mostrar los resultados
        //}

        //public async Task<IActionResult> SociosConMayorUtilidad(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    // Consulta para obtener la utilidad total por socio en el rango de fechas especificado
        //    var resultado = await _context.Utilidades
        //        .Where(u => u.fechaUtilidad >= fechaInicio && u.fechaUtilidad <= fechaFin)
        //        .GroupBy(u => u.socioID)
        //        .Select(g => new
        //        {
        //            SocioID = g.Key,
        //            SocioNombre = g.FirstOrDefault().socio,
        //            UtilidadTotal = g.Sum(u => u.utilidadPorSocio)
        //        })
        //        .OrderByDescending(s => s.UtilidadTotal)
        //        .ToListAsync();

        //    return View(resultado);
        //}

        //public async Task<IActionResult> SociosConMayorUtilidad(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    // Consulta para obtener la utilidad total por socio en el rango de fechas especificado
        //    var resultado = await _context.Utilidades
        //        .Where(u => u.fechaUtilidad >= fechaInicio && u.fechaUtilidad <= fechaFin)
        //        .GroupBy(u => u.socioID)
        //        .Select(g => new
        //        {
        //            SocioID = g.Key,
        //            SocioNombre = g.FirstOrDefault().socio,
        //            UtilidadTotal = g.Sum(u => u.utilidadPorSocio)
        //        })
        //        .OrderByDescending(s => s.UtilidadTotal)
        //        .FirstOrDefaultAsync(); // Obtener solo el socio con la mayor utilidad

        //    // Verificar si hay resultados
        //    if (resultado == null)
        //    {
        //        ViewBag.NoResultsMessage = "No se encontraron socios con utilidad en el rango de fechas especificado.";
        //        return View();
        //    }

        //    return View(resultado); // Pasar solo el socio con mayor utilidad
        //}

        //public async Task<IActionResult> SociosConMayorUtilidad(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    // Consulta para obtener la utilidad total por socio en el rango de fechas especificado
        //    var resultado = await _context.Utilidades
        //        .Where(u => u.fechaUtilidad >= fechaInicio && u.fechaUtilidad <= fechaFin)
        //        .GroupBy(u => u.socioID)
        //        .Select(g => new
        //        {
        //            SocioID = g.Key,
        //            SocioNombre = g.FirstOrDefault().socio, // Asumiendo que 'socio' tiene el nombre del socio
        //            UtilidadTotal = g.Sum(u => u.utilidadPorSocio)
        //        })
        //        .OrderByDescending(s => s.UtilidadTotal) // Ordena por la mayor utilidad
        //        //.ToListAsync();
        //        .FirstOrDefaultAsync(); // Obtener solo el primer socio, el que tiene la mayor utilidad

        //    // Verificar si hay resultados
        //    if (resultado == null)
        //    {
        //        ViewBag.NoResultsMessage = "No se encontraron socios con utilidad en el rango de fechas especificado.";
        //        return View();
        //    }

        //    return View(resultado); // Pasar solo el socio con mayor utilidad
        //}

        //public async Task<IActionResult> SociosConMayorUtilidad(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    // Consulta para obtener la utilidad total por socio en el rango de fechas especificado
        //    var resultado = await _context.Utilidades
        //        .Where(u => u.fechaUtilidad >= fechaInicio && u.fechaUtilidad <= fechaFin)
        //        .GroupBy(u => u.socioID)
        //        .Select(g => new
        //        {
        //            SocioID = g.Key,
        //            SocioNombre = g.FirstOrDefault().socio, // Asumiendo que 'socio' tiene el nombre del socio
        //            UtilidadTotal = g.Sum(u => u.utilidadPorSocio)
        //        })
        //        .OrderBy(s => s.UtilidadTotal) // Ordena por la mayor utilidad
        //        .Take(5) // Obtén los 5 socios con mayores utilidades
        //        .ToListAsync(); // Ejecuta la consulta y obtiene los resultados

        //    // Verificar si hay resultados
        //    if (resultado == null || !resultado.Any())
        //    {
        //        ViewBag.NoResultsMessage = "No se encontraron socios con utilidad en el rango de fechas especificado.";
        //        return View();
        //    }

        //    return View(resultado); // Pasar la lista de los 5 socios con mayores utilidades
        //}


        public async Task<IActionResult> SociosConMayorUtilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            // Ajustar las fechas para manejar solo la parte del día (sin horas)
            var fechaInicioSinHora = fechaInicio.Date;
            var fechaFinSinHora = fechaFin.Date.AddDays(1).AddMilliseconds(-1); 

            // Consulta para obtener los socios con utilidades
            var resultado = await _context.Utilidades
                .Where(u => u.fechaUtilidad >= fechaInicioSinHora && u.fechaUtilidad <= fechaFinSinHora) 
                .GroupBy(u => u.socioID)
                .Select(g => new
                {
                    SocioID = g.Key,
                    SocioNombre = g.FirstOrDefault().socio, 
                    UtilidadTotal = g.Sum(u => u.utilidadPorSocio) 
                })
                .Where(g => _context.Socios.Any(s => s.socioID == g.SocioID)) 
                .OrderByDescending(s => s.UtilidadTotal) 
                .Take(5) 
                .ToListAsync(); 

            // Verificar si hay resultados
            if (resultado == null || !resultado.Any())
            {
                ViewBag.NoResultsMessage = "No se encontraron socios con utilidad en el rango de fechas especificado.";
                return View();
            }

            return View(resultado); 
        }



    }

}

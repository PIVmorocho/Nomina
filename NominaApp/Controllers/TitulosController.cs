using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Controllers
{
    /// <summary>Histórico de títulos por empleado.</summary>
    [Authorize(Policy = "RRHH")]
    public class TitulosController : Controller
    {
        private readonly AppDbContext _db;
        public TitulosController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? empNo, int pagina = 1)
        {
            const int porPagina = 20;
            var query = _db.Titles.Include(t => t.Employee).AsQueryable();
            if (empNo.HasValue) query = query.Where(t => t.EmpNo == empNo.Value);

            var total = await query.CountAsync();
            var items = await query.OrderBy(t => t.Employee!.LastName)
                .Skip((pagina - 1) * porPagina).Take(porPagina).ToListAsync();

            ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
            ViewBag.FiltroEmp = empNo;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPags = (int)Math.Ceiling(total / (double)porPagina);
            return View(items);
        }

        public async Task<IActionResult> Agregar()
        {
            ViewBag.Empleados = new SelectList(
                await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(),
                "EmpNo", "FullName");
            return View("Form", new Title { FromDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(Title model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
                return View("Form", model);
            }

            if (model.ToDate.HasValue && model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("ToDate", "La fecha hasta no puede ser anterior a la fecha desde.");
                ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
                return View("Form", model);
            }

            // Sin solapamiento del mismo título
            var solapa = await _db.Titles.AnyAsync(t =>
                t.EmpNo == model.EmpNo && t.TitleName == model.TitleName &&
                t.FromDate < (model.ToDate ?? DateTime.MaxValue) &&
                (t.ToDate == null || t.ToDate > model.FromDate));

            if (solapa)
            {
                ModelState.AddModelError("", "Ya existe ese título en el período indicado.");
                ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
                return View("Form", model);
            }

            _db.Titles.Add(model);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Título registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TerminarVigencia(int empNo, string title, DateTime fromDate)
        {
            var t = await _db.Titles.FindAsync(empNo, title, fromDate);
            if (t == null) return NotFound();
            t.ToDate = DateTime.Today;
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Vigencia del título terminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Controllers
{
    /// <summary>Gerentes de departamento con vigencias.</summary>
    [Authorize(Policy = "RRHH")]
    public class GerentesController : Controller
    {
        private readonly AppDbContext _db;
        public GerentesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? deptNo, int pagina = 1)
        {
            const int porPagina = 20;
            var query = _db.DeptManagers
                .Include(d => d.Employee)
                .Include(d => d.Department)
                .AsQueryable();

            if (deptNo.HasValue) query = query.Where(d => d.DeptNo == deptNo.Value);

            var total = await query.CountAsync();
            var items = await query.OrderBy(d => d.Department!.DeptName)
                .Skip((pagina - 1) * porPagina).Take(porPagina).ToListAsync();

            ViewBag.Departamentos = new SelectList(await _db.Departments.Where(d => d.Activo).OrderBy(d => d.DeptName).ToListAsync(), "DeptNo", "DeptName");
            ViewBag.FiltroDept = deptNo;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPags = (int)Math.Ceiling(total / (double)porPagina);
            return View(items);
        }

        public async Task<IActionResult> Asignar()
        {
            await CargarListasAsync();
            return View("Form", new DeptManager { FromDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(DeptManager model)
        {
            if (!ModelState.IsValid) { await CargarListasAsync(); return View("Form", model); }

            if (model.ToDate.HasValue && model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("ToDate", "La fecha hasta no puede ser anterior a la fecha desde.");
                await CargarListasAsync(); return View("Form", model);
            }

            // Solo un gerente activo por departamento
            var hayActivo = await _db.DeptManagers.AnyAsync(d =>
                d.DeptNo == model.DeptNo &&
                d.FromDate < (model.ToDate ?? DateTime.MaxValue) &&
                (d.ToDate == null || d.ToDate > model.FromDate));

            if (hayActivo)
            {
                ModelState.AddModelError("", "Ya existe un gerente activo para ese departamento en ese período.");
                await CargarListasAsync(); return View("Form", model);
            }

            _db.DeptManagers.Add(model);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Gerente asignado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TerminarVigencia(int empNo, int deptNo, DateTime fromDate)
        {
            var mgr = await _db.DeptManagers.FindAsync(empNo, deptNo, fromDate);
            if (mgr == null) return NotFound();
            mgr.ToDate = DateTime.Today;
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Vigencia del gerente terminada.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
            ViewBag.Departamentos = new SelectList(await _db.Departments.Where(d => d.Activo).OrderBy(d => d.DeptName).ToListAsync(), "DeptNo", "DeptName");
        }
    }
}
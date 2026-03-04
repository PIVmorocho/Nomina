using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Controllers
{
    /// <summary>Asignaciones empleado–departamento con vigencias.</summary>
    [Authorize(Policy = "RRHH")]
    public class AsignacionesController : Controller
    {
        private readonly AppDbContext _db;
        public AsignacionesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? empNo, int? deptNo, int pagina = 1)
        {
            const int porPagina = 20;
            var query = _db.DeptEmps
                .Include(d => d.Employee)
                .Include(d => d.Department)
                .AsQueryable();

            if (empNo.HasValue) query = query.Where(d => d.EmpNo == empNo.Value);
            if (deptNo.HasValue) query = query.Where(d => d.DeptNo == deptNo.Value);

            var total = await query.CountAsync();
            var items = await query.OrderBy(d => d.Employee!.LastName)
                .Skip((pagina - 1) * porPagina).Take(porPagina).ToListAsync();

            ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
            ViewBag.Departamentos = new SelectList(await _db.Departments.Where(d => d.Activo).OrderBy(d => d.DeptName).ToListAsync(), "DeptNo", "DeptName");
            ViewBag.FiltroEmp = empNo;
            ViewBag.FiltroDept = deptNo;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPags = (int)Math.Ceiling(total / (double)porPagina);
            return View(items);
        }

        public async Task<IActionResult> Agregar()
        {
            await CargarListasAsync();
            return View("Form", new DeptEmp { FromDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(DeptEmp model)
        {
            if (!ModelState.IsValid) { await CargarListasAsync(); return View("Form", model); }

            if (model.ToDate.HasValue && model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("ToDate", "La fecha hasta no puede ser anterior a la fecha desde.");
                await CargarListasAsync(); return View("Form", model);
            }

            // Verificar solapamiento
            var solapa = await _db.DeptEmps.AnyAsync(d =>
                d.EmpNo == model.EmpNo &&
                d.FromDate < (model.ToDate ?? DateTime.MaxValue) &&
                (d.ToDate == null || d.ToDate > model.FromDate));

            if (solapa)
            {
                ModelState.AddModelError("", "El empleado ya tiene una asignación activa en ese período.");
                await CargarListasAsync(); return View("Form", model);
            }

            _db.DeptEmps.Add(model);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Asignación registrada.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Asignaciones/TerminarVigencia
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TerminarVigencia(int empNo, int deptNo, DateTime fromDate)
        {
            var asig = await _db.DeptEmps.FindAsync(empNo, deptNo, fromDate);
            if (asig == null) return NotFound();
            asig.ToDate = DateTime.Today;
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Vigencia terminada.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
            ViewBag.Departamentos = new SelectList(await _db.Departments.Where(d => d.Activo).OrderBy(d => d.DeptName).ToListAsync(), "DeptNo", "DeptName");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;
using NominaApp.Services;

namespace NominaApp.Controllers
{
    /// <summary>Gestión de salarios con auditoría.</summary>
    [Authorize(Policy = "RRHH")]
    public class SalariosController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AuditoriaService _auditoria;

        public SalariosController(AppDbContext db, AuditoriaService auditoria)
        {
            _db = db;
            _auditoria = auditoria;
        }

        public async Task<IActionResult> Index(int? empNo, int pagina = 1)
        {
            const int porPagina = 20;
            var query = _db.Salaries.Include(s => s.Employee).AsQueryable();
            if (empNo.HasValue) query = query.Where(s => s.EmpNo == empNo.Value);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(s => s.FromDate)
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
            return View("Form", new Salary { FromDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(Salary model)
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

            // Solo un salario activo por fecha
            var solapa = await _db.Salaries.AnyAsync(s =>
                s.EmpNo == model.EmpNo &&
                s.FromDate < (model.ToDate ?? DateTime.MaxValue) &&
                (s.ToDate == null || s.ToDate > model.FromDate));

            if (solapa)
            {
                ModelState.AddModelError("", "El empleado ya tiene un salario activo en ese período.");
                ViewBag.Empleados = new SelectList(await _db.Employees.Where(e => e.Activo).OrderBy(e => e.LastName).ToListAsync(), "EmpNo", "FullName");
                return View("Form", model);
            }

            _db.Salaries.Add(model);
            await _db.SaveChangesAsync();

            var usuario = User.Identity?.Name ?? "sistema";
            var emp = await _db.Employees.FindAsync(model.EmpNo);
            await _auditoria.RegistrarAsync(
                usuario, model.EmpNo, model.SalaryAmount,
                $"Nuevo salario registrado para {emp?.FullName} desde {model.FromDate:dd/MM/yyyy}");

            TempData["Exito"] = "Salario registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TerminarVigencia(int empNo, DateTime fromDate)
        {
            var sal = await _db.Salaries.FindAsync(empNo, fromDate);
            if (sal == null) return NotFound();
            sal.ToDate = DateTime.Today;
            await _db.SaveChangesAsync();

            var usuario = User.Identity?.Name ?? "sistema";
            await _auditoria.RegistrarAsync(usuario, empNo, sal.SalaryAmount,
                $"Salario finalizado al {DateTime.Today:dd/MM/yyyy}");

            TempData["Exito"] = "Vigencia del salario terminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
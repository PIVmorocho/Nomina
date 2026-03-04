using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Controllers
{
    /// <summary>ABM de departamentos.</summary>
    [Authorize(Policy = "RRHH")]
    public class DepartamentosController : Controller
    {
        private readonly AppDbContext _db;

        public DepartamentosController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            const int porPagina = 20;

            var query = _db.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(d => d.DeptName.ToLower().Contains(buscar.ToLower()));

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.DeptName)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            ViewBag.Buscar = buscar;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPags = (int)Math.Ceiling(total / (double)porPagina);

            return View(items);
        }

        // GET: /Departamentos/Crear
        public IActionResult Crear()
        {
            return View("Form", new Department());
        }

        // POST: /Departamentos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Department model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            _db.Departments.Add(model);
            await _db.SaveChangesAsync();

            TempData["Exito"] = "Departamento creado.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Departamentos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var dept = await _db.Departments.FindAsync(id);

            if (dept == null)
                return NotFound();

            return View("Form", dept);
        }

        // POST: /Departamentos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Department model)
        {
            if (id != model.DeptNo)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Form", model);

            _db.Departments.Update(model);
            await _db.SaveChangesAsync();

            TempData["Exito"] = "Departamento actualizado.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var dept = await _db.Departments.FindAsync(id);

            if (dept == null)
                return NotFound();

            dept.Activo = false;

            await _db.SaveChangesAsync();

            TempData["Exito"] = "Departamento desactivado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Controllers
{
    /// <summary>ABM de empleados.</summary>
    [Authorize(Policy = "RRHH")]
    public class EmpleadosController : Controller
    {
        private readonly AppDbContext _db;
        public EmpleadosController(AppDbContext db) => _db = db;

        // GET: /Empleados
        public async Task<IActionResult> Index(string? buscar, int pagina = 1)
        {
            const int porPagina = 20;
            var query = _db.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim().ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(buscar) ||
                    e.LastName.ToLower().Contains(buscar) ||
                    e.Ci.ToLower().Contains(buscar) ||
                    (e.Correo != null && e.Correo.ToLower().Contains(buscar)));
            }

            var total = await query.CountAsync();
            var items = await query.OrderBy(e => e.LastName)
                .Skip((pagina - 1) * porPagina).Take(porPagina).ToListAsync();

            ViewBag.Buscar = buscar;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPags = (int)Math.Ceiling(total / (double)porPagina);
            return View(items);
        }

        // GET: /Empleados/Crear
        public IActionResult Crear() => View("Form", new Employee { HireDate = DateTime.Today });

        // POST: /Empleados/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Employee model)
        {
            if (!ModelState.IsValid) return View("Form", model);

            if (await _db.Employees.AnyAsync(e => e.Ci == model.Ci))
            {
                ModelState.AddModelError("Ci", "Ya existe un empleado con esa CI.");
                return View("Form", model);
            }
            if (!string.IsNullOrWhiteSpace(model.Correo) &&
                await _db.Employees.AnyAsync(e => e.Correo == model.Correo))
            {
                ModelState.AddModelError("Correo", "El correo ya está en uso.");
                return View("Form", model);
            }

            _db.Employees.Add(model);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Empleado creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Empleados/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            return View("Form", emp);
        }

        // POST: /Empleados/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Employee model)
        {
            if (id != model.EmpNo) return BadRequest();
            if (!ModelState.IsValid) return View("Form", model);

            if (await _db.Employees.AnyAsync(e => e.Ci == model.Ci && e.EmpNo != id))
            {
                ModelState.AddModelError("Ci", "Ya existe otro empleado con esa CI.");
                return View("Form", model);
            }
            if (!string.IsNullOrWhiteSpace(model.Correo) &&
                await _db.Employees.AnyAsync(e => e.Correo == model.Correo && e.EmpNo != id))
            {
                ModelState.AddModelError("Correo", "El correo ya está en uso por otro empleado.");
                return View("Form", model);
            }

            _db.Employees.Update(model);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Empleado actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Empleados/Ver/5
        public async Task<IActionResult> Ver(int id)
        {
            var emp = await _db.Employees
                .Include(e => e.DeptEmps).ThenInclude(d => d.Department)
                .Include(e => e.Titles)
                .Include(e => e.Salaries)
                .FirstOrDefaultAsync(e => e.EmpNo == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        // POST: /Empleados/Desactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            emp.Activo = false;
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Empleado desactivado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
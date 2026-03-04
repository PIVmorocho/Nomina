using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Controllers
{
    /// <summary>Administración de usuarios (solo Administrador).</summary>
    [Authorize(Policy = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _db;

        public UsuariosController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var users = await _db.Users
                .Include(u => u.Employee)
                .OrderBy(u => u.Usuario)
                .ToListAsync();

            return View(users);
        }

        // GET: /Usuarios/Crear
        public async Task<IActionResult> Crear()
        {
            ViewBag.Empleados = new SelectList(
                await _db.Employees
                    .Where(e => e.Activo)
                    .OrderBy(e => e.LastName)
                    .ToListAsync(),
                "EmpNo", "FullName");

            ViewBag.Roles = new SelectList(new[] { "Administrador", "RRHH" });

            return View("Form", new User());
        }

        // POST: /Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(User model, string claveTexto)
        {
            ModelState.Remove("Clave");

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(claveTexto))
            {
                if (string.IsNullOrWhiteSpace(claveTexto))
                    ModelState.AddModelError("", "La clave es obligatoria.");

                ViewBag.Empleados = new SelectList(
                    await _db.Employees
                        .Where(e => e.Activo)
                        .OrderBy(e => e.LastName)
                        .ToListAsync(),
                    "EmpNo", "FullName");

                ViewBag.Roles = new SelectList(new[] { "Administrador", "RRHH" });

                return View("Form", model);
            }

            if (await _db.Users.AnyAsync(u => u.Usuario == model.Usuario))
            {
                ModelState.AddModelError("Usuario", "Ya existe ese usuario.");

                ViewBag.Empleados = new SelectList(
                    await _db.Employees
                        .Where(e => e.Activo)
                        .OrderBy(e => e.LastName)
                        .ToListAsync(),
                    "EmpNo", "FullName");

                ViewBag.Roles = new SelectList(new[] { "Administrador", "RRHH" });

                return View("Form", model);
            }

            model.Clave = BCrypt.Net.BCrypt.HashPassword(claveTexto);

            _db.Users.Add(model);

            await _db.SaveChangesAsync();

            TempData["Exito"] = "Usuario creado.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetClave(string usuario, string nuevaClave)
        {
            var user = await _db.Users.FindAsync(usuario);

            if (user == null)
                return NotFound();

            user.Clave = BCrypt.Net.BCrypt.HashPassword(nuevaClave);

            await _db.SaveChangesAsync();

            TempData["Exito"] = "Clave actualizada.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(string usuario)
        {
            var user = await _db.Users.FindAsync(usuario);

            if (user == null)
                return NotFound();

            user.Activo = false;

            await _db.SaveChangesAsync();

            TempData["Exito"] = "Usuario desactivado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
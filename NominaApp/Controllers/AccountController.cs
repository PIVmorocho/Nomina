using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NominaApp.Data;
using NominaApp.ViewModels;
using System.Security.Claims;

namespace NominaApp.Controllers
{
    /// <summary>Controlador de autenticación.</summary>
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db) => _db = db;

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _db.Users.FirstOrDefault(u => u.Usuario == model.Usuario && u.Activo);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Clave, user.Clave))
            {
                ModelState.AddModelError("", "Usuario o clave incorrectos.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  user.Usuario),
                new Claim(ClaimTypes.Role,  user.Rol),
                new Claim("EmpNo",          user.EmpNo.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties
            {
                IsPersistent = model.Recordar,
                ExpiresUtc = model.Recordar
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
            return RedirectToAction("Index", "Dashboard");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Acceso() => View();


        // =========================================================
        // SOLO PARA CONFIGURAR EL USUARIO ADMIN - BORRAR DESPUÉS
        // =========================================================
        [AllowAnonymous]
        public IActionResult SetupAdmin()
        {
            var user = _db.Users.FirstOrDefault(u => u.Usuario == "admin");

            string hash = BCrypt.Net.BCrypt.HashPassword("Admin123");

            if (user == null)
            {
                var emp = _db.Employees.FirstOrDefault();

                if (emp == null)
                {
                    var nuevoEmp = new NominaApp.Models.Employee
                    {
                        Ci = "0000000001",
                        BirthDate = new DateTime(1980, 1, 1),
                        FirstName = "Admin",
                        LastName = "Sistema",
                        Gender = "M",
                        HireDate = DateTime.Today,
                        Correo = "admin@nomina.com",
                        Activo = true
                    };

                    _db.Employees.Add(nuevoEmp);
                    _db.SaveChanges();
                    emp = nuevoEmp;
                }

                _db.Users.Add(new NominaApp.Models.User
                {
                    Usuario = "admin",
                    EmpNo = emp.EmpNo,
                    Clave = hash,
                    Rol = "Administrador",
                    Activo = true
                });

                _db.SaveChanges();

                return Content($"✅ Usuario admin creado. Hash: {hash}");
            }
            else
            {
                user.Clave = hash;
                user.Rol = "Administrador";
                user.Activo = true;

                _db.SaveChanges();

                return Content($"✅ Hash actualizado. Nuevo hash: {hash}");
            }
        }
    }
}
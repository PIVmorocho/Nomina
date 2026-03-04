using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.ViewModels;

namespace NominaApp.Controllers
{
    /// <summary>Controlador del panel principal.</summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;

            var vm = new DashboardViewModel
            {
                TotalEmpleados = await _db.Employees.CountAsync(e => e.Activo),
                TotalDepartamentos = await _db.Departments.CountAsync(d => d.Activo),
                SalarioVigenteMes = await _db.Salaries
                    .Where(s => s.FromDate <= hoy && (s.ToDate == null || s.ToDate >= hoy))
                    .SumAsync(s => (long?)s.SalaryAmount) ?? 0,
                VigenciasPorVencer = await _db.DeptEmps
                    .Where(d => d.ToDate != null && d.ToDate >= hoy && d.ToDate <= hoy.AddDays(30))
                    .CountAsync()
            };

            // Resumen por departamento
            vm.ResumenPorDepto = await _db.Departments
                .Where(d => d.Activo)
                .Select(d => new ResumenDepto
                {
                    Departamento = d.DeptName,
                    Empleados = d.DeptEmps.Count(e => e.ToDate == null || e.ToDate >= hoy),
                    SalarioTotal = d.DeptEmps
                        .Where(e => e.ToDate == null || e.ToDate >= hoy)
                        .SelectMany(e => e.Employee!.Salaries
                            .Where(s => s.FromDate <= hoy && (s.ToDate == null || s.ToDate >= hoy)))
                        .Sum(s => (long?)s.SalaryAmount) ?? 0
                })
                .ToListAsync();

            return View(vm);
        }
    }
}
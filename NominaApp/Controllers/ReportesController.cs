using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;
using NominaApp.ViewModels;

namespace NominaApp.Controllers
{
    /// <summary>Generación de reportes de nómina.</summary>
    [Authorize(Policy = "RRHH")]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _db;
        public ReportesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.Departamentos = new SelectList(
                await _db.Departments.Where(d => d.Activo).OrderBy(d => d.DeptName).ToListAsync(),
                "DeptNo", "DeptName");
            return View(new FiltroReporteViewModel { FechaDesde = DateTime.Today.AddMonths(-1), FechaHasta = DateTime.Today });
        }

        // ── Nómina Vigente ────────────────────────────────────────────────────
        public async Task<IActionResult> NominaVigente(int? deptNo, string? formato)
        {
            var hoy = DateTime.Today;
            var query = _db.Employees.Where(e => e.Activo)
                .Include(e => e.DeptEmps).ThenInclude(d => d.Department)
                .Include(e => e.Titles)
                .Include(e => e.Salaries)
                .AsQueryable();

            if (deptNo.HasValue)
                query = query.Where(e => e.DeptEmps.Any(d => d.DeptNo == deptNo.Value &&
                    (d.ToDate == null || d.ToDate >= hoy)));

            var empleados = await query.ToListAsync();

            var items = empleados.Select(e => new ReporteNominaItem
            {
                EmpNo = e.EmpNo,
                Empleado = e.FullName,
                Depto = e.DeptEmps.Where(d => d.ToDate == null || d.ToDate >= hoy)
                            .Select(d => d.Department?.DeptName).FirstOrDefault() ?? "-",
                Titulo = e.Titles.Where(t => t.ToDate == null || t.ToDate >= hoy)
                            .OrderByDescending(t => t.FromDate)
                            .Select(t => t.TitleName).FirstOrDefault() ?? "-",
                Salario = e.Salaries.Where(s => s.FromDate <= hoy && (s.ToDate == null || s.ToDate >= hoy))
                            .Select(s => (long?)s.SalaryAmount).FirstOrDefault() ?? 0,
                Desde = e.Salaries.Where(s => s.FromDate <= hoy && (s.ToDate == null || s.ToDate >= hoy))
                            .Select(s => (DateTime?)s.FromDate).FirstOrDefault() ?? DateTime.MinValue
            }).ToList();

            if (formato == "excel") return ExportarNominaExcel(items);

            ViewBag.DeptNo = deptNo;
            return View(items);
        }

        // ── Cambios Salariales ────────────────────────────────────────────────
        public async Task<IActionResult> CambiosSalariales(DateTime? desde, DateTime? hasta, string? formato)
        {
            desde ??= DateTime.Today.AddMonths(-1);
            hasta ??= DateTime.Today;

            var items = await _db.LogAuditoriaSalarios
                .Include(l => l.Employee)
                .Where(l => l.FechaActualizacion >= desde && l.FechaActualizacion <= hasta)
                .OrderByDescending(l => l.FechaActualizacion)
                .Select(l => new ReporteCambioItem
                {
                    EmpNo = l.EmpNo,
                    Empleado = l.Employee != null ? l.Employee.FirstName + " " + l.Employee.LastName : l.EmpNo.ToString(),
                    Usuario = l.Usuario,
                    Fecha = l.FechaActualizacion,
                    Detalle = l.DetalleCambio,
                    Salario = l.Salario
                }).ToListAsync();

            if (formato == "excel") return ExportarCambiosExcel(items);

            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            return View(items);
        }

        // ── Estructura Organizacional ─────────────────────────────────────────
        public async Task<IActionResult> EstructuraOrganizacional(string? formato)
        {
            var hoy = DateTime.Today;
            var items = await _db.Departments.Where(d => d.Activo)
                .Include(d => d.DeptManagers).ThenInclude(m => m.Employee)
                .Include(d => d.DeptEmps).ThenInclude(e => e.Employee)
                    .ThenInclude(e => e!.Titles)
                .Include(d => d.DeptEmps).ThenInclude(e => e.Employee)
                    .ThenInclude(e => e!.Salaries)
                .SelectMany(d => d.DeptEmps
                    .Where(e => e.ToDate == null || e.ToDate >= hoy)
                    .Select(e => new ReporteEstructuraItem
                    {
                        Departamento = d.DeptName,
                        Gerente = d.DeptManagers
                            .Where(m => m.ToDate == null || m.ToDate >= hoy)
                            .Select(m => m.Employee != null ? m.Employee.FirstName + " " + m.Employee.LastName : "-")
                            .FirstOrDefault() ?? "-",
                        Empleado = e.Employee != null ? e.Employee.FirstName + " " + e.Employee.LastName : "-",
                        Titulo = e.Employee!.Titles
                            .Where(t => t.ToDate == null || t.ToDate >= hoy)
                            .OrderByDescending(t => t.FromDate)
                            .Select(t => t.TitleName).FirstOrDefault() ?? "-",
                        Salario = e.Employee.Salaries
                            .Where(s => s.FromDate <= hoy && (s.ToDate == null || s.ToDate >= hoy))
                            .Select(s => (long?)s.SalaryAmount).FirstOrDefault() ?? 0
                    }))
                .OrderBy(r => r.Departamento).ThenBy(r => r.Empleado)
                .ToListAsync();

            if (formato == "excel") return ExportarEstructuraExcel(items);
            return View(items);
        }

        // ── Exportar Excel helpers ────────────────────────────────────────────
        private FileContentResult ExportarNominaExcel(List<ReporteNominaItem> items)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Nómina Vigente");
            string[] headers = { "Emp#", "Empleado", "Departamento", "Título", "Salario", "Desde" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];
            ws.Row(1).Style.Font.Bold = true;
            for (int i = 0; i < items.Count; i++)
            {
                var r = items[i];
                ws.Cell(i + 2, 1).Value = r.EmpNo;
                ws.Cell(i + 2, 2).Value = r.Empleado;
                ws.Cell(i + 2, 3).Value = r.Depto;
                ws.Cell(i + 2, 4).Value = r.Titulo;
                ws.Cell(i + 2, 5).Value = r.Salario;
                ws.Cell(i + 2, 6).Value = r.Desde.ToString("dd/MM/yyyy");
            }
            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"NominaVigente_{DateTime.Today:yyyyMMdd}.xlsx");
        }

        private FileContentResult ExportarCambiosExcel(List<ReporteCambioItem> items)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Cambios Salariales");
            string[] headers = { "Emp#", "Empleado", "Usuario", "Fecha", "Salario", "Detalle" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];
            ws.Row(1).Style.Font.Bold = true;
            for (int i = 0; i < items.Count; i++)
            {
                var r = items[i];
                ws.Cell(i + 2, 1).Value = r.EmpNo;
                ws.Cell(i + 2, 2).Value = r.Empleado;
                ws.Cell(i + 2, 3).Value = r.Usuario;
                ws.Cell(i + 2, 4).Value = r.Fecha.ToString("dd/MM/yyyy");
                ws.Cell(i + 2, 5).Value = r.Salario;
                ws.Cell(i + 2, 6).Value = r.Detalle;
            }
            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"CambiosSalariales_{DateTime.Today:yyyyMMdd}.xlsx");
        }

        private FileContentResult ExportarEstructuraExcel(List<ReporteEstructuraItem> items)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Estructura Organizacional");
            string[] headers = { "Departamento", "Gerente", "Empleado", "Título", "Salario" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];
            ws.Row(1).Style.Font.Bold = true;
            for (int i = 0; i < items.Count; i++)
            {
                var r = items[i];
                ws.Cell(i + 2, 1).Value = r.Departamento;
                ws.Cell(i + 2, 2).Value = r.Gerente;
                ws.Cell(i + 2, 3).Value = r.Empleado;
                ws.Cell(i + 2, 4).Value = r.Titulo;
                ws.Cell(i + 2, 5).Value = r.Salario;
            }
            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"EstructuraOrg_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}
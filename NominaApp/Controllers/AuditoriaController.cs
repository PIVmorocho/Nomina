using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaApp.Data;

namespace NominaApp.Controllers
{
    /// <summary>Consulta del log de auditoría salarial.</summary>
    [Authorize(Policy = "RRHH")]
    public class AuditoriaController : Controller
    {
        private readonly AppDbContext _db;
        public AuditoriaController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(int? empNo, string? usuario, DateTime? desde, DateTime? hasta, int pagina = 1)
        {
            const int porPagina = 20;
            var query = _db.LogAuditoriaSalarios.Include(l => l.Employee).AsQueryable();

            if (empNo.HasValue) query = query.Where(l => l.EmpNo == empNo.Value);
            if (!string.IsNullOrWhiteSpace(usuario)) query = query.Where(l => l.Usuario.Contains(usuario));
            if (desde.HasValue) query = query.Where(l => l.FechaActualizacion >= desde.Value);
            if (hasta.HasValue) query = query.Where(l => l.FechaActualizacion <= hasta.Value);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(l => l.FechaActualizacion)
                .Skip((pagina - 1) * porPagina).Take(porPagina).ToListAsync();

            ViewBag.FiltroEmp = empNo;
            ViewBag.Usuario = usuario;
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPags = (int)Math.Ceiling(total / (double)porPagina);
            return View(items);
        }

        // GET: /Auditoria/ExportarExcel
        public async Task<IActionResult> ExportarExcel(int? empNo, string? usuario, DateTime? desde, DateTime? hasta)
        {
            var query = _db.LogAuditoriaSalarios.Include(l => l.Employee).AsQueryable();
            if (empNo.HasValue) query = query.Where(l => l.EmpNo == empNo.Value);
            if (!string.IsNullOrWhiteSpace(usuario)) query = query.Where(l => l.Usuario.Contains(usuario));
            if (desde.HasValue) query = query.Where(l => l.FechaActualizacion >= desde.Value);
            if (hasta.HasValue) query = query.Where(l => l.FechaActualizacion <= hasta.Value);

            var items = await query.OrderByDescending(l => l.FechaActualizacion).ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Auditoría Salarios");
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Usuario";
            ws.Cell(1, 3).Value = "Fecha";
            ws.Cell(1, 4).Value = "Empleado";
            ws.Cell(1, 5).Value = "Salario";
            ws.Cell(1, 6).Value = "Detalle";
            ws.Row(1).Style.Font.Bold = true;

            for (int i = 0; i < items.Count; i++)
            {
                var r = items[i];
                ws.Cell(i + 2, 1).Value = r.Id;
                ws.Cell(i + 2, 2).Value = r.Usuario;
                ws.Cell(i + 2, 3).Value = r.FechaActualizacion.ToString("dd/MM/yyyy");
                ws.Cell(i + 2, 4).Value = r.Employee?.FullName ?? r.EmpNo.ToString();
                ws.Cell(i + 2, 5).Value = r.Salario;
                ws.Cell(i + 2, 6).Value = r.DetalleCambio;
            }
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"AuditoriaSalarios_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}
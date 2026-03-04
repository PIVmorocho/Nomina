using NominaApp.Data;
using NominaApp.Models;

namespace NominaApp.Services
{
    /// <summary>Servicio para registrar auditoría de salarios.</summary>
    public class AuditoriaService
    {
        private readonly AppDbContext _db;

        public AuditoriaService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>Registra un evento de auditoría salarial.</summary>
        public async Task RegistrarAsync(string usuario, int empNo, long salario, string detalle)
        {
            var log = new LogAuditoriaSalarios
            {
                Usuario = usuario,
                FechaActualizacion = DateTime.Today,
                DetalleCambio = detalle,
                Salario = salario,
                EmpNo = empNo
            };
            _db.LogAuditoriaSalarios.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
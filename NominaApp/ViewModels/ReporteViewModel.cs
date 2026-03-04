using System.ComponentModel.DataAnnotations;

namespace NominaApp.ViewModels
{
    public class ReporteNominaItem
    {
        public int EmpNo { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public string Depto { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public long Salario { get; set; }
        public DateTime Desde { get; set; }
    }

    public class ReporteCambioItem
    {
        public int EmpNo { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public long Salario { get; set; }
    }

    public class ReporteEstructuraItem
    {
        public string Departamento { get; set; } = string.Empty;
        public string Gerente { get; set; } = string.Empty;
        public string Empleado { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public long Salario { get; set; }
    }

    public class FiltroReporteViewModel
    {
        [Display(Name = "Desde")]
        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Hasta")]
        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        [Display(Name = "Departamento")]
        public int? DeptNo { get; set; }

        public string Tipo { get; set; } = "nomina"; // nomina | cambios | estructura
    }
}
namespace NominaApp.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmpleados { get; set; }
        public int TotalDepartamentos { get; set; }
        public long SalarioVigenteMes { get; set; }
        public int VigenciasPorVencer { get; set; }

        public List<ResumenDepto> ResumenPorDepto { get; set; } = new();
    }

    public class ResumenDepto
    {
        public string Departamento { get; set; } = string.Empty;
        public int Empleados { get; set; }
        public long SalarioTotal { get; set; }
    }
}